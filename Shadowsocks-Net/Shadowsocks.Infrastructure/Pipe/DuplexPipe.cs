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
    using static ReadWriteResult;

    /// <summary>
    /// A duplex pipe with filterable <see cref="IReader"/> / <see cref="IWriter"/> support.
    /// </summary>
    public sealed class DuplexPipe : IDuplexPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion


        #region IDuplexPipe
        public IClientReaderAccessor Reader 
        {
            get 
            {
                //Throw.IfNull(()=>_pipeA2B);
                if(null!=_pipeA2B && _pipeA2B.Reader.Client==value)
            } 
        }
        public IClientReaderAccessor Writer => _writerPair;

        public IClient ClientA
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
                        _readerPair.ReaderA = new ClientReader(value, false, _bufferSize, _logger);
                        _writerPair.WriterA = new ClientWriter(value, true, _bufferSize, _logger);
                    }
                }
            }
        }
        public IClient ClientB
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
                        _readerPair.ReaderB = new ClientReader(value, false, _bufferSize, _logger);
                        _writerPair.WriterB = new ClientWriter(value, true, _bufferSize, _logger);
                    }
                }
            }
        }
        #endregion


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

        SimplexPipe _pipeA2B = null, _pipeB2A = null;


        int _bufferSize = 8192;
        CancellationTokenSource _cancellation = null;
        Task _taskPipeA2B = null, _taskPipeB2A = null;
        object _clientLinkLock = new object();

        ILogger _logger = null;
        public DuplexPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
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

        public DuplexPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null, params ClientFilter[] filters)
            : this(clientA, clientB, bufferSize, logger)
        {
            this.AddClientFilter(filters);
        }

        public DuplexPipe(ClientReader readerA, ClientWriter writerA, ClientReader readerB, ClientWriter writerB, ILogger logger = null)
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

        public DuplexPipe(int? bufferSize = 8192, ILogger logger = null)
            : base()
        {
            _bufferSize = bufferSize ?? Defaults.ReceiveBufferSize;
            _logger = logger;

            _readerPair = new ClientReaderPair();
            _writerPair = new ClientWriterPair();
        }

        ~DuplexPipe()
        {
            StopPipe();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void StartPipe(CancellationToken cancellationToken)
        {
            StopPipe();
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
        public override void StopPipe()
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

        public DuplexPipe AddClientFilter(ClientFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if ((null == ClientA && null == ClientB) || (filter.Client != ClientA && filter.Client != ClientB))
            {
                throw new InvalidOperationException("no matched client for this filter.");
            }

            (Reader[filter.Client] as ClientReader).AddFilter(filter);
            (Writer[filter.Client] as ClientWriter).AddFilter(filter);

            return this;
        }
        public void AddClientFilter(IEnumerable<ClientFilter> filters)//TODO lock
        {
            foreach (var f in filters)
            {
                AddClientFilter(f);
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
