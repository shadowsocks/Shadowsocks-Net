/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
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

    /// <summary>
    /// A duplex pipe.
    /// </summary>
    public sealed class DefaultPipe : IPipe
    {
        public IClient ClientA { get; private set; }
        public IClient ClientB { get; private set; }

        public IReadOnlyCollection<PipeFilter> FiltersA => _filtersA;
        public IReadOnlyCollection<PipeFilter> FiltersB => _filtersB;

        public event EventHandler<PipeBrokenEventArgs> OnBroken;


        ILogger _logger = null;
        SortedSet<PipeFilter> _filtersA = null;
        SortedSet<PipeFilter> _filtersB = null;
        SpinLock _filterExecLock = default;
        CancellationTokenSource _cancellation = null;

        int _bufferSize = Defaults.ReceiveBufferSize;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientA"></param>
        /// <param name="clientB"></param>
        /// <param name="bufferSize">1500 is enough for UDP</param>
        /// <param name="logger"></param>
        public DefaultPipe(IClient clientA, IClient clientB, int? bufferSize = 8192, ILogger logger = null)
        {
            ClientA = Throw.IfNull(() => clientA);
            ClientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => clientA, clientB);

            _bufferSize = bufferSize ?? Defaults.ReceiveBufferSize;

            _filtersA = new SortedSet<PipeFilter>();
            _filtersB = new SortedSet<PipeFilter>();

            _filterExecLock = new SpinLock(true);

            _logger = logger;
        }

        ~DefaultPipe()
        {
            UnPipe();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Pipe()
        {
            UnPipe();

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
            //using (var received = SmartBuffer.Rent(_bufferSize))
            var received = SmartBuffer.Rent(_bufferSize);

            while (!cancellationToken.IsCancellationRequested)
            {
                received.SignificantLength = await ClientA.ReadAsync(received.Memory, cancellationToken);
                _logger.LogInformation($"received {received.SignificantLength} bytes from [{ClientA.EndPoint.ToString()}].");

                if (0 >= received.SignificantLength)
                {
                    ReportBroken(PipeBrokenCause.Exception);
                    return;
                }
                if (_filtersA.Count > 0)
                {
                    var result = ExecuteFilter_AfterReading(ClientA, received, _filtersA, cancellationToken);
                    received.Dispose();
                    received = result.Buffer;
                    if (!result.Continue)
                    {
                        _logger.LogInformation($"pipe broke by filterA [{ClientA.EndPoint.ToString()}].");
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        return;
                    }
                }
                if (_filtersB.Count > 0)
                {
                    var result = ExecuteFilter_BeforeWriting(ClientB, received, _filtersB, cancellationToken);
                    received.Dispose();
                    received = result.Buffer;
                    if (!result.Continue)
                    {
                        _logger.LogInformation($"pipe broke by filterB [{ClientB.EndPoint.ToString()}].");
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        return;
                    }
                }
                if (null != received && received.SignificantLength > 0)
                {
                    _logger?.LogInformation($"{received.SignificantLength} bytes left after filtering.");
                    int written = await ClientB.WriteAsync(received.SignificanMemory, cancellationToken);

                    _logger?.LogInformation($"Pipe [{ClientA.EndPoint.ToString()}] to [{ClientB.EndPoint.ToString()}] {written} bytes.");
                    if (0 >= written)
                    {
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.Exception);
                        return;
                    }
                }
                //continue piping
            }//end while
            received?.Dispose();
            ReportBroken(PipeBrokenCause.Cancelled);

        }
        async Task PipeB2A(CancellationToken cancellationToken)
        {
            //using (var received = SmartBuffer.Rent(_bufferSize))
            var received = SmartBuffer.Rent(_bufferSize);

            while (!cancellationToken.IsCancellationRequested)
            {
                received.SignificantLength = await ClientB.ReadAsync(received.Memory, cancellationToken);
                _logger.LogInformation($"received {received.SignificantLength} bytes from [{ClientB.EndPoint.ToString()}].");

                if (0 >= received.SignificantLength)
                {
                    ReportBroken(PipeBrokenCause.Exception);
                    return;
                }
                if (_filtersB.Count > 0)
                {
                    var result = ExecuteFilter_AfterReading(ClientB, received, _filtersB, cancellationToken);
                    received.Dispose();
                    received = result.Buffer;
                    if (!result.Continue)
                    {
                        _logger.LogInformation($"pipe broke by filterB [{ClientB.EndPoint.ToString()}].");
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        return;
                    }
                }
                if (_filtersA.Count > 0)
                {
                    var result = ExecuteFilter_BeforeWriting(ClientA, received, _filtersA, cancellationToken);
                    received.Dispose();
                    received = result.Buffer;
                    if (!result.Continue)
                    {
                        _logger.LogInformation($"pipe broke by filterA [{ClientA.EndPoint.ToString()}].");
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.FilterBreak);
                        return;
                    }
                }
                if (null != received && received.SignificantLength > 0)
                {
                    _logger?.LogInformation($"{received.SignificantLength} bytes left after filtering.");
                    int written = await ClientA.WriteAsync(received.SignificanMemory, cancellationToken);
                    _logger?.LogInformation($"Pipe [{ClientB.EndPoint.ToString()}] to [{ClientA.EndPoint.ToString()}] {written} bytes.");

                    if (0 >= written)
                    {
                        received?.Dispose();
                        ReportBroken(PipeBrokenCause.Exception);
                        return;
                    }
                }
                //continue piping
            }//end while
            received?.Dispose();
            ReportBroken(PipeBrokenCause.Cancelled);

        }

        PipeFilterResult ExecuteFilter_AfterReading(IClient client, SmartBuffer memory, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = memory;
            bool @continue = true;
            foreach (var filter in filters)
            {
                try
                {
                    var result = filter.AfterReading(new PipeFilterContext(client, prevFilterMemory.SignificanMemory));
                    prevFilterMemory.Dispose();
                    prevFilterMemory = result.Buffer;
                    @continue = result.Continue;
                    if (!result.Continue) { break; }
                    if (cancellationToken.IsCancellationRequested) { break; }
                }
                catch (Exception ex)
                {
                    @continue = false;
                    _logger?.LogError(ex, $"ExecuteFilter_AfterReading [{client.EndPoint.ToString()}].");
                }
            }
            return new PipeFilterResult(client, prevFilterMemory, @continue);
        }
        PipeFilterResult ExecuteFilter_BeforeWriting(IClient client, SmartBuffer memory, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = memory;
            bool @continue = true;
            foreach (var filter in filters.Reverse())
            {
                try
                {
                    var result = filter.BeforeWriting(new PipeFilterContext(client, prevFilterMemory.SignificanMemory));
                    prevFilterMemory.Dispose();
                    prevFilterMemory = result.Buffer;
                    @continue = result.Continue;
                    if (!result.Continue) { break; }
                    if (cancellationToken.IsCancellationRequested) { break; }
                }
                catch (Exception ex)
                {
                    @continue = false;
                    _logger?.LogError(ex, $"ExecuteFilter_BeforeWriting [{client.EndPoint.ToString()}].");
                }
            }
            return new PipeFilterResult(client, prevFilterMemory, @continue);
        }

        void DoFilter_Lock(ref bool locked)
        {
            if (!_filterExecLock.IsHeldByCurrentThread)
            {
                _filterExecLock.Enter(ref locked);
                _logger?.LogInformation("Pipe _filterExecLock.Enter().");
            }
        }
        void DoFilter_UnLock(ref bool locked)
        {
            if (locked)
            {
                _filterExecLock.Exit();
                locked = false;
                _logger?.LogInformation("Pipe _filterExecLock.Exit().");
            }
        }

        public DefaultPipe ApplyFilter(PipeFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if (filter.Client == ClientA && !_filtersA.Contains(filter))
            {
                _filtersA.Add(filter);
                return this;
            }
            if (filter.Client == ClientB && !_filtersB.Contains(filter))
            {
                _filtersB.Add(filter);
            }
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

    }
}
