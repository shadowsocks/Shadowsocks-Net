/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    using static ClientReadWriteResult;

    /// <summary>
    /// A duplex pipe with filter support.
    /// </summary>
    public sealed class DefaultPipe : DuplexPipe
    {
        public override IClientReaderAccessor Reader => _readerPair;
        public override IClientWriterAccessor Writer => _writerPair;


        ClientReaderPair _readerPair = null;
        ClientWriterPair _writerPair = null;

        int _bufferSize = 8192;
        CancellationTokenSource _cancellation = null;

        AutoResetEvent _pipeA2BWaitHandle = new AutoResetEvent(true), _pipeB2AWaitHandle = new AutoResetEvent(true);
        volatile bool _isPiping = false;
        object _clientLinkLock = new object();

        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
            : base(clientA, clientB)
        {
            _bufferSize = bufferSize ?? Defaults.ReceiveBufferSize;
            _logger = logger;

            _readerPair = new ClientReaderPair(
                   new ClientReader(ClientA, bufferSize, logger),
                   new ClientReader(ClientB, bufferSize, logger));

            _writerPair = new ClientWriterPair(
                new ClientWriter(ClientA, bufferSize, logger),
                new ClientWriter(ClientB, bufferSize, logger));

        }

        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null, params ClientFilter[] filters)
            : this(clientA, clientB, bufferSize, logger)
        {
            this.ApplyFilter(filters);
        }

        public DefaultPipe(ClientReader readerA, ClientWriter writerA, ClientReader readerB, ClientWriter writerB, ILogger logger = null)
           : base(readerA, writerA, readerB, writerB)
        {
            _logger = logger;

            _readerPair = new ClientReaderPair(readerA, readerB);
            _writerPair = new ClientWriterPair(writerA, writerB);
        }


        public DefaultPipe(int? bufferSize = 8192, ILogger logger = null)
            : base()
        {
            _readerPair = new ClientReaderPair();
            _writerPair = new ClientWriterPair();
        }

        public override void LinkupClientA(IClient client)
        {
            lock (_clientLinkLock)
            {
                if (_isPiping) { throw new InvalidOperationException("Can not link up client while piping."); }
                else
                {
                    Throw.IfNull(() => client);
                    Throw.IfEqualsTo(() => client, ClientB);


                    ClientA = client;
                    _readerPair.ReaderA = new ClientReader(client, _bufferSize, _logger);
                    _writerPair.WriterA = new ClientWriter(client, _bufferSize, _logger);
                }
            }
        }
        public override void LinkupClientB(IClient client)
        {
            lock (_clientLinkLock)
            {
                if (_isPiping) { throw new InvalidOperationException("Can not link up client while piping."); }
                else
                {
                    Throw.IfNull(() => client);
                    Throw.IfEqualsTo(() => client, ClientA);

                    ClientB = client;
                    _readerPair.ReaderB = new ClientReader(client, _bufferSize, _logger);
                    _writerPair.WriterB = new ClientWriter(client, _bufferSize, _logger);
                }
            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Pipe(CancellationToken cancellationToken)
        {
            UnPipe();
            lock (_clientLinkLock)
            {
                _pipeA2BWaitHandle.WaitOne();
                _pipeB2AWaitHandle.WaitOne();


                _cancellation ??= new CancellationTokenSource();

                var lnkCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellation.Token);

                Task.Run(async () => { await PipeA2B(lnkCts.Token); });
                Task.Run(async () => { await PipeB2A(lnkCts.Token); });

                _isPiping = true;
            }
        }
        public override void UnPipe()
        {
            if (null != _cancellation)
            {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;

            }

            _pipeA2BWaitHandle.WaitOne();
            _pipeB2AWaitHandle.WaitOne();

            _pipeA2BWaitHandle.Set();
            _pipeB2AWaitHandle.Set();
            _isPiping = false;
        }


        async Task PipeA2B(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var readerResult = await Reader[ClientA].Read(cancellationToken);
                if (Failed == readerResult.Result)
                {
                    ReportBroken(PipeBrokenCause.Exception);
                    break;
                }
                if (BrokeByFilter == readerResult.Result)
                {
                    _logger?.LogInformation($"Pipe broke by filterA [{ClientA.EndPoint.ToString()}].");
                    ReportBroken(PipeBrokenCause.FilterBreak);
                    break;
                }

                if (readerResult.Read > 0)
                {
                    var writeResult = await Writer[ClientB].Write(readerResult.Memory.SignificantMemory, cancellationToken);
                    if (Failed == writeResult.Result)
                    {
                        ReportBroken(PipeBrokenCause.Exception);
                        break;
                    }
                    if (BrokeByFilter == writeResult.Result)
                    {
                        _logger?.LogInformation($"Pipe broke by filterB [{ClientB.EndPoint.ToString()}].");
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        break;
                    }
                    _logger?.LogInformation($"Pipe [{ClientA.EndPoint.ToString()}] => [{ClientB.EndPoint.ToString()}] {writeResult.Written} bytes.");
                    ReportPiping(new PipingEventArgs { Bytes = writeResult.Written, Origin = ClientA.EndPoint, Destination = ClientB.EndPoint });
                }
                readerResult.Memory?.Dispose();
            }//continue piping
            if (cancellationToken.IsCancellationRequested) { ReportBroken(PipeBrokenCause.Cancelled); }
            _pipeA2BWaitHandle.Set();
        }
        async Task PipeB2A(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var readerResult = await Reader[ClientB].Read(cancellationToken);

                if (Failed == readerResult.Result)
                {
                    ReportBroken(PipeBrokenCause.Exception);
                    break;
                }
                if (BrokeByFilter == readerResult.Result)
                {
                    _logger?.LogInformation($"Pipe broke by ClientB [{ClientB.EndPoint.ToString()}].");
                    ReportBroken(PipeBrokenCause.FilterBreak);
                    break;
                }
                if (readerResult.Read > 0)
                {
                    var writeResult = await Writer[ClientA].Write(readerResult.Memory.SignificantMemory, cancellationToken);
                    if (Failed == writeResult.Result)
                    {
                        ReportBroken(PipeBrokenCause.Exception);
                        break;
                    }
                    if (BrokeByFilter == writeResult.Result)
                    {
                        _logger?.LogInformation($"Pipe broke by ClientA [{ClientA.EndPoint.ToString()}].");
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        break;
                    }
                    _logger?.LogInformation($"Pipe [{ClientB.EndPoint.ToString()}] => [{ClientA.EndPoint.ToString()}] {writeResult.Written} bytes.");
                    ReportPiping(new PipingEventArgs { Bytes = writeResult.Written, Origin = ClientB.EndPoint, Destination = ClientA.EndPoint });
                }
                readerResult.Memory?.Dispose();
            } //continue piping
            if (cancellationToken.IsCancellationRequested) { ReportBroken(PipeBrokenCause.Cancelled); }
            _pipeB2AWaitHandle.Set();
        }

        public DefaultPipe ApplyFilter(ClientFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            (Reader[filter.Client] as ClientReader).ApplyFilter(filter);
            (Writer[filter.Client] as ClientWriter).ApplyFilter(filter);

            return this;
        }
        public void ApplyFilter(IEnumerable<ClientFilter> filters)//TODO lock
        {
            foreach (var f in filters)
            {
                ApplyFilter(f);
            }
        }


    }
}
