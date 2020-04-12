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
                        _clientAReaderWriterPair.Set(_clientA, _bufferSize, _logger);

                        CreatePipe();
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
                        _clientBReaderWriterPair.Set(_clientB, _bufferSize, _logger);

                        CreatePipe();
                    }
                }
            }
        }

        public IReader GetReader(IClient client)
        {
            Throw.IfNull(() => client);
            if (object.ReferenceEquals(client, _clientAReaderWriterPair.Client)) { return _clientAReaderWriterPair.Reader; }
            if (object.ReferenceEquals(client, _clientBReaderWriterPair.Client)) { return _clientBReaderWriterPair.Reader; }
            return null;
        }
        public IWriter GetWriter(IClient client)
        {
            Throw.IfNull(() => client);
            if (object.ReferenceEquals(client, _clientAReaderWriterPair.Client)) { return _clientAReaderWriterPair.Writer; }
            if (object.ReferenceEquals(client, _clientBReaderWriterPair.Client)) { return _clientBReaderWriterPair.Writer; }
            return null;
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


        IClient _clientA = null, _clientB = null;
        ClientReaderWriterPair _clientAReaderWriterPair = null, _clientBReaderWriterPair = null;

        SimplexPipe _pipeA2B = null, _pipeB2A = null;
        Task _taskPipeA2B = null, _taskPipeB2A = null;

        int _bufferSize = 8192;
        CancellationTokenSource _cancellation = null;
        object _clientLinkLock = new object();

        ILogger _logger = null;
        public DuplexPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
            : this(bufferSize, logger)
        {
            _clientA = Throw.IfNull(() => clientA);
            _clientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => _clientA, _clientB);

            _clientAReaderWriterPair.Set(_clientA, _bufferSize, _logger);
            _clientBReaderWriterPair.Set(_clientB, _bufferSize, _logger);

            CreatePipe();
        }

        public DuplexPipe(int? bufferSize = 8192, ILogger logger = null)
        {
            _clientAReaderWriterPair = new ClientReaderWriterPair();
            _clientBReaderWriterPair = new ClientReaderWriterPair();

            _bufferSize = bufferSize ?? Defaults.ReceiveBufferSize;
            _logger = logger;
        }

        ~DuplexPipe()
        {
            StopPipe();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartPipe(CancellationToken cancellationToken)
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

                    _taskPipeA2B = Task.Run(async () => { await DoPipe(_pipeA2B, lnkCts.Token); }, lnkCts.Token);
                    _taskPipeB2A = Task.Run(async () => { await DoPipe(_pipeB2A, lnkCts.Token); }, lnkCts.Token);
                }
            }
        }
        public void StopPipe()
        {
            if (null != _cancellation)
            {
                _cancellation.Cancel();
                ////_cancellation.Dispose();// UnPipe() by both tasks concurrently
                ////_cancellation = null;
            }
        }


        public DuplexPipe AddFilter(IClient client, ClientFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);
            Throw.IfNull(() => client);

            if ((null == ClientA && null == ClientB) || (client != ClientA && client != ClientB))
            {
                throw new InvalidOperationException("no matched client for this filter.");
            }

            filter.Client = client;

            (GetReader(client) as ClientReader)?.AddFilter(filter);
            (GetWriter(client) as ClientWriter)?.AddFilter(filter);

            return this;
        }
        public void AddFilter(IClient client, IEnumerable<ClientFilter> filters)//TODO lock
        {
            foreach (var f in filters)
            {
                AddFilter(client, f);
            }
        }

        async Task DoPipe(SimplexPipe pipe, CancellationToken cancellationToken)
        {           
            while (!cancellationToken.IsCancellationRequested)
            {
                var r = await pipe.Pipe(cancellationToken);               
                if (r.Broken)
                {                   
                    ReportBroken(r.BrokenCause);
                    break;
                }
                else
                {                   
                    ReportPiping(new PipingEventArgs { Bytes = r.BytesPiped, Origin = pipe.Reader.Client.EndPoint, Destination = pipe.Writer.Client.EndPoint });
                }
            }//continue piping
            if (cancellationToken.IsCancellationRequested) { ReportBroken(PipeBrokenCause.Cancelled); }
        }


        void CreatePipe()
        {
            if (null != _clientA && null != _clientB)
            {
                Throw.IfEqualsTo(() => _clientA, _clientB);

                _pipeA2B = new SimplexPipe(_clientAReaderWriterPair.Reader, _clientBReaderWriterPair.Writer, _logger);
                _pipeB2A = new SimplexPipe(_clientBReaderWriterPair.Reader, _clientAReaderWriterPair.Writer, _logger);
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
