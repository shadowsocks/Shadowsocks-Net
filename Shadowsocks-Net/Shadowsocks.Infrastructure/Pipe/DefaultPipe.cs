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
    /// A duplex pipe with filterable <see cref="IClientReader"/> / <see cref="IClientWriter"/> support.
    /// </summary>
    public sealed class DefaultPipe : DuplexPipe
    {
        public override IClientReaderAccessor Reader => _readerPair;
        public override IClientWriterAccessor Writer => _writerPair;

        public override IClient ClientA
        {
            get => _clientA;
            set
            {
                lock (_clientLinkLock)
                {
                    if (IsPiping) { throw new InvalidOperationException("Can not link up client while piping."); }
                    else
                    {
                        Throw.IfNull(() => value);
                        Throw.IfEqualsTo(() => value, _clientB);


                        _clientA = value;
                        _readerPair.ReaderA = new ClientReader(value, _bufferSize, _logger);
                        _writerPair.WriterA = new ClientWriter(value, _bufferSize, _logger);
                    }
                }
            }
        }
        public override IClient ClientB
        {
            get => _clientB;
            set
            {
                lock (_clientLinkLock)
                {
                    if (IsPiping) { throw new InvalidOperationException("Can not link up client while piping."); }
                    else
                    {
                        Throw.IfNull(() => value);
                        Throw.IfEqualsTo(() => value, _clientA);

                        _clientB = value;
                        _readerPair.ReaderB = new ClientReader(value, _bufferSize, _logger);
                        _writerPair.WriterB = new ClientWriter(value, _bufferSize, _logger);
                    }
                }
            }
        }


        /// <summary>
        /// Indicates whether the pipe is piping data now.
        /// </summary>
        public bool IsPiping
        {
            get
            {
                bool running = false;
                if (null != _taskPipeA2B)
                {
                    running |= !(_taskPipeA2B.IsCompleted || _taskPipeA2B.IsCanceled || _taskPipeA2B.IsFaulted || _taskPipeA2B.IsCompletedSuccessfully);
                }
                if (null != _taskPipeB2A)
                {
                    running |= !(_taskPipeB2A.IsCompleted || _taskPipeB2A.IsCanceled || _taskPipeB2A.IsFaulted || _taskPipeB2A.IsCompletedSuccessfully);
                }
                return running;
            }
        }


        IClient _clientA = null;
        IClient _clientB = null;

        ClientReaderPair _readerPair = null;
        ClientWriterPair _writerPair = null;

        int _bufferSize = 8192;
        CancellationTokenSource _cancellation = null;
        Task _taskPipeA2B = null, _taskPipeB2A = null;
        object _clientLinkLock = new object();

        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
            : base(clientA, clientB)
        {
            _clientA = Throw.IfNull(() => clientA);
            _clientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => _clientA, _clientB);

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
            this.ApplyClientFilter(filters);
        }

        public DefaultPipe(ClientReader readerA, ClientWriter writerA, ClientReader readerB, ClientWriter writerB, ILogger logger = null)
           : base(readerA, writerA, readerB, writerB)
        {
            Throw.IfNull(() => readerA);
            Throw.IfNull(() => writerA);
            Throw.IfNotEqualsTo(() => readerA.Client, writerA.Client);

            Throw.IfNull(() => readerB);
            Throw.IfNull(() => writerB);
            Throw.IfNotEqualsTo(() => readerB.Client, writerB.Client);

            Throw.IfEqualsTo(() => readerA.Client, readerB.Client);
            Throw.IfEqualsTo(() => writerA.Client, writerB.Client);

            _clientA = readerA.Client;
            _clientB = readerB.Client;


            _logger = logger;

            _readerPair = new ClientReaderPair(readerA, readerB);
            _writerPair = new ClientWriterPair(writerA, writerB);
        }

        public DefaultPipe(int? bufferSize = 8192, ILogger logger = null)
            : base()
        {
            _bufferSize = bufferSize ?? Defaults.ReceiveBufferSize;
            _logger = logger;

            _readerPair = new ClientReaderPair();
            _writerPair = new ClientWriterPair();
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Pipe(CancellationToken cancellationToken)
        {
            UnPipe();
            if (null != _taskPipeA2B)
            {
                _taskPipeA2B.Wait();
                _taskPipeA2B.Dispose();
                _taskPipeA2B = null;
            }
            if (null != _taskPipeB2A)
            {
                _taskPipeB2A.Wait();
                _taskPipeB2A.Dispose();
                _taskPipeB2A = null;
            }

            lock (_clientLinkLock)
            {
                if (null == ClientA || null == ClientB)
                {
                    throw new InvalidOperationException("Can not pipe until both clients are linked up.");
                }
                else
                {
                    _cancellation?.Cancel();
                    _cancellation?.Dispose();
                    _cancellation = new CancellationTokenSource();

                    var lnkCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellation.Token);

                    _taskPipeA2B = Task.Run(async () => { await PipeA2B(lnkCts.Token); }, lnkCts.Token);
                    _taskPipeB2A = Task.Run(async () => { await PipeB2A(lnkCts.Token); }, lnkCts.Token);
                }
            }
        }
        public override void UnPipe()
        {
            if (null != _cancellation)
            {
                _cancellation.Cancel();
                ////_cancellation.Dispose();// UnPipe() by both tasks concurrently
                ////_cancellation = null;
            }
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

                if (readerResult.Read > 0)//happens sometimes, [AfterReading] filter may not return data.
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

        }

        public DefaultPipe ApplyClientFilter(ClientFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if((null==ClientA && null==ClientB) || (filter.Client!=ClientA && filter.Client !=ClientB))
            {
                throw new InvalidOperationException("no matched client for this filter.");
            }

            (Reader[filter.Client] as ClientReader).ApplyFilter(filter);
            (Writer[filter.Client] as ClientWriter).ApplyFilter(filter);

            return this;
        }
        public void ApplyClientFilter(IEnumerable<ClientFilter> filters)//TODO lock
        {
            foreach (var f in filters)
            {
                ApplyClientFilter(f);
            }
        }


    }
}
