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
    using static PipeReadWriteResult;

    /// <summary>
    /// A duplex pipe.
    /// </summary>
    public sealed class DefaultPipe : IPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion

        public IClient ClientA { get; private set; }
        public IClient ClientB { get; private set; }

        public IReadOnlyDictionary<IClient, PipeReader> PipeReader => _pipeReader;
        public IReadOnlyDictionary<IClient, PipeWriter> PipeWriter => _pipeWriter;

        Dictionary<IClient, PipeReader> _pipeReader = null;
        Dictionary<IClient, PipeWriter> _pipeWriter = null;

        CancellationTokenSource _cancellation = null;

        AutoResetEvent _pipeA2BMutex = new AutoResetEvent(true), _pipeB2AMutex = new AutoResetEvent(true);
        ILogger _logger = null;


        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
        {
            ClientA = Throw.IfNull(() => clientA);
            ClientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => clientA, clientB);

            _pipeReader = new Dictionary<IClient, PipeReader>();
            _pipeWriter = new Dictionary<IClient, PipeWriter>();


            _pipeReader.Add(ClientA, new PipeReader(ClientA, bufferSize, logger));
            _pipeReader.Add(ClientB, new PipeReader(ClientB, bufferSize, logger));

            _pipeWriter.Add(ClientA, new PipeWriter(ClientA, bufferSize, logger));
            _pipeWriter.Add(ClientB, new PipeWriter(ClientB, bufferSize, logger));

            _logger = logger;

        }

        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null, params PipeFilter[] filters)
            : this(clientA, clientB, bufferSize, logger)
        {
            this.ApplyFilter(filters);
        }


        ~DefaultPipe()
        {
            UnPipe();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Pipe()
        {
            UnPipe();

            _pipeA2BMutex.WaitOne();
            _pipeB2AMutex.WaitOne();

            _cancellation ??= new CancellationTokenSource();

            Task.Run(async () => { await PipeA2B(_cancellation.Token); });
            Task.Run(async () => { await PipeB2A(_cancellation.Token); });
        }
        public void UnPipe()
        {
            if (null != _cancellation)
            {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
        }


        async Task PipeA2B(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var readerResult = await _pipeReader[ClientA].Read(cancellationToken);
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
                    var writeResult = await _pipeWriter[ClientB].Write(readerResult.Memory.SignificantMemory, cancellationToken);
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
                //continue piping
            }//end while
            if (cancellationToken.IsCancellationRequested) { ReportBroken(PipeBrokenCause.Cancelled); }
            _pipeA2BMutex.Set();
        }
        async Task PipeB2A(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var readerResult = await _pipeReader[ClientB].Read(cancellationToken);

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
                    var writeResult = await _pipeWriter[ClientA].Write(readerResult.Memory.SignificantMemory, cancellationToken);
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
                //continue piping
            }//end while 
            if (cancellationToken.IsCancellationRequested) { ReportBroken(PipeBrokenCause.Cancelled); }
            _pipeB2AMutex.Set();
        }

        public DefaultPipe ApplyFilter(PipeFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            PipeReader[filter.Client].ApplyFilter(filter);
            PipeWriter[filter.Client].ApplyFilter(filter);

            return this;
        }
        public void ApplyFilter(IEnumerable<PipeFilter> filters)//TODO lock
        {
            foreach (var f in filters)
            {
                ApplyFilter(f);
            }
        }

        void ReportBroken(PipeBrokenCause cause, PipeException exception = null)
        {
            try
            {
                if (null != OnBroken)
                {
                    OnBroken(this, new PipeBrokenEventArgs()
                    {
                        Pipe = this,
                        Cause = cause,
                        Exception = exception
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Pipe ReportBroken error.");
            }
        }
        void ReportPiping(PipingEventArgs pipingEventArgs)
        {
            try
            {
                if (null != OnPiping)
                {
                    OnPiping(this, pipingEventArgs);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Pipe ReportPiping error.");
            }
        }
    }
}
