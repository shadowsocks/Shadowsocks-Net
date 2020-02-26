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

        public event EventHandler<PipeEventArgs> OnBroken;


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


        public void Pipe()
        {
            UnPipe();

            _cancellation ??= new CancellationTokenSource();
            var tA2B = Task.Run(async () => { await PipeA2B(_cancellation.Token); }, _cancellation.Token);
            var tB2A = Task.Run(async () => { await PipeB2A(_cancellation.Token); }, _cancellation.Token);

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


        async Task PipeA2B(CancellationToken cancellationToken)
        {
            SmartBuffer buffer = SmartBuffer.Rent(_bufferSize);
            buffer.SignificantLength = await ClientA.ReadAsync(buffer.Memory, cancellationToken);
            if (buffer.SignificantLength > 0)
            {
                if (_filtersA.Count > 0)
                {
                    bool locked = false;
                    try
                    {
                        DoFilter_Lock(ref locked);

                        PipeFilterResult filterResult = DoFilter_AfterReading(ref buffer, _filtersA, cancellationToken);
                        if (!filterResult.Continue || cancellationToken.IsCancellationRequested)
                        {
                            ReportBroken();
                            return;
                        }
                        else
                        {
                            buffer = filterResult.Buffer;
                        }

                        DoFilter_UnLock(ref locked);
                    }
                    finally
                    {
                        DoFilter_UnLock(ref locked);
                    }
                }//end FilterA
                if (_filtersB.Count > 0)
                {
                    bool locked = false;
                    try
                    {
                        DoFilter_Lock(ref locked);

                        PipeFilterResult filterResult = DoFilter_BeforeWriting(ref buffer, _filtersB, cancellationToken);
                        if (!filterResult.Continue || cancellationToken.IsCancellationRequested)
                        {
                            ReportBroken();
                            return;
                        }
                        else
                        {
                            buffer = filterResult.Buffer;
                        }

                        DoFilter_UnLock(ref locked);
                    }
                    finally
                    {
                        DoFilter_UnLock(ref locked);
                    }
                }//end FilterB

                int written = await ClientB.WriteAsync(buffer.Memory.Slice(0, buffer.SignificantLength), cancellationToken);
                _logger?.LogInformation($"DefaultPipe Pipe A to B {written} bytes.");
                if (written < 0)
                {
                    //ClientB.Close();
                    buffer.Dispose();
                    ReportBroken();
                    return;
                }
            }
            else if (0 == buffer.SignificantLength)
            {
                _logger?.LogInformation($"DefaultPipe read = 0.");
            }
            else//<0
            {
                _logger?.LogInformation($"DefaultPipe error, read={buffer.SignificantLength}.");
                ClientA.Close();
                ReportBroken();
                return;
            }


            buffer.Dispose();

            if (!cancellationToken.IsCancellationRequested)
            {
                await PipeA2B(cancellationToken);//continue.
            }
        }
        async Task PipeB2A(CancellationToken cancellationToken)
        {
            SmartBuffer buffer = SmartBuffer.Rent(_bufferSize);//read buff
            buffer.SignificantLength = await ClientB.ReadAsync(buffer.Memory, cancellationToken);


            if (buffer.SignificantLength > 0)
            {
                if (_filtersB.Count > 0)
                {
                    bool locked = false;
                    try
                    {
                        DoFilter_Lock(ref locked);

                        PipeFilterResult filterResult = DoFilter_AfterReading(ref buffer, _filtersB, cancellationToken);
                        if (!filterResult.Continue || cancellationToken.IsCancellationRequested)
                        {
                            ReportBroken();
                            return;
                        }
                        else
                        {
                            buffer = filterResult.Buffer;
                        }

                        DoFilter_UnLock(ref locked);
                    }
                    finally
                    {
                        DoFilter_UnLock(ref locked);
                    }
                }//end FilterB
                if (_filtersA.Count > 0)
                {
                    bool locked = false;
                    try
                    {
                        DoFilter_Lock(ref locked);

                        PipeFilterResult filterResult = DoFilter_BeforeWriting(ref buffer, _filtersA, cancellationToken);
                        if (!filterResult.Continue || cancellationToken.IsCancellationRequested)
                        {
                            ReportBroken();
                            return;
                        }
                        else
                        {
                            buffer = filterResult.Buffer;
                        }

                        DoFilter_UnLock(ref locked);
                    }
                    finally
                    {
                        DoFilter_UnLock(ref locked);
                    }
                }//end FilterA

                int written = await ClientA.WriteAsync(buffer.Memory.Slice(0, buffer.SignificantLength), cancellationToken);//Pipe
                _logger?.LogInformation($"DefaultPipe Pipe B to A {written} bytes.");
                if (written < 0)
                {
                    ClientA.Close();
                    buffer.Dispose();
                    ReportBroken();
                    return;
                }

            }
            else if (0 == buffer.SignificantLength)
            {
                _logger?.LogInformation($"DefaultPipe read = 0.");
            }
            else//<0
            {
                _logger?.LogInformation($"DefaultPipe error, read={buffer.SignificantLength}.");
                ClientB.Close();
                ReportBroken();
                return;
            }


            buffer.Dispose();//free memory.

            if (!cancellationToken.IsCancellationRequested)
            {
                await PipeB2A(cancellationToken);//continue.
            }
        }


        PipeFilterResult DoFilter_AfterReading(ref SmartBuffer buffer, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            PipeFilterResult rt = default;
            foreach (var f in filters)
            {
                _logger?.LogInformation($"DefaultPipe DoFilter_AfterReading {f.ToString()}...");
                PipeFilterContext ctx = new PipeFilterContext(f.Client, buffer.Memory.Slice(0, buffer.SignificantLength));
                try
                {
                    rt = f.AfterReading(ctx);
                    if (!rt.Continue || cancellationToken.IsCancellationRequested)
                    {
                        buffer.Dispose();
                        rt.Buffer?.Dispose();
                        break;
                    }
                    buffer.Dispose();
                    buffer = rt.Buffer;
                    //next filter
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"DefaultPipe DoFilter_AfterReading {f.ToString()} error.");
                    buffer.Dispose();
                    rt = new PipeFilterResult(ctx.Client, null, false);
                    break;
                }
            }

            return rt;

        }
        PipeFilterResult DoFilter_BeforeWriting(ref SmartBuffer buffer, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            PipeFilterResult rt = default;
            foreach (var f in filters)
            {
                _logger?.LogInformation($"DefaultPipe DoFilter_BeforeWriting {f.ToString()}...");
                PipeFilterContext ctx = new PipeFilterContext(f.Client, buffer.Memory.Slice(0, buffer.SignificantLength));
                try
                {
                    rt = f.BeforeWriting(ctx);
                    if (!rt.Continue || cancellationToken.IsCancellationRequested)
                    {
                        buffer.Dispose();
                        rt.Buffer?.Dispose();
                        break;
                    }
                    buffer.Dispose();
                    buffer = rt.Buffer;
                    //next filter
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"DefaultPipe DoFilter_BeforeWriting {f.ToString()} error.");
                    buffer.Dispose();
                    rt = new PipeFilterResult(ctx.Client, null, false);
                    break;
                }
            }

            return rt;
        }



        void DoFilter_Lock(ref bool locked)
        {
            if (!_filterExecLock.IsHeldByCurrentThread)
            {
                _filterExecLock.Enter(ref locked);
                _logger?.LogDebug("DefaultPipe _filterExecLock.Enter().");
            }
        }
        void DoFilter_UnLock(ref bool locked)
        {
            if (locked)
            {
                _filterExecLock.Exit();
                locked = false;
                _logger?.LogDebug("DefaultPipe _filterExecLock.Exit().");
            }
        }


        void ReportBroken(PipeException exception = null)
        {
            try
            {
                if (null != OnBroken)
                {
                    OnBroken(this, new PipeEventArgs() { Pipe = this });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "DefaultPipe ReportBroken error.");
            }
        }

    }
}
