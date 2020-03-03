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
    using static PipeReadWriteResult;
    public class PipeWriter
    {
        IClient _client = null;
        SortedSet<PipeFilter> _filters = null;
        int _bufferSize = 8192;
        ILogger _logger = null;

        public PipeWriter(IClient client, int? bufferSize = 8192, ILogger logger = null)
        {
            _client = Throw.IfNull(() => client);
            _filters = new SortedSet<PipeFilter>();
            _bufferSize = bufferSize ?? 8192;

            _logger = logger;
        }
        public async ValueTask<PipeWriteResult> Write(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
        {
            if (data.IsEmpty)
            {
                _logger?.LogWarning($"PipeWriter Try to write empty memory. [{_client.EndPoint.ToString()}].");
                return new PipeWriteResult(Failed, 0);
            }

            ReadOnlyMemory<byte> toWrite = data;
            SmartBuffer filterResultBuffer = null; 
            if (_filters.Count > 0)
            {
                var result = ExecuteFilter_BeforeWriting(_client, data, _filters, cancellationToken);
                if (!result.Continue)
                {
                    result.Buffer?.Dispose();
                    _logger?.LogInformation($"Pipe broke by filter [{_client.EndPoint.ToString()}].");
                    return new PipeWriteResult(BrokeByFilter, 0);
                }
                else
                {
                    filterResultBuffer = result.Buffer;
                    toWrite = null != filterResultBuffer ? filterResultBuffer.SignificantMemory : ReadOnlyMemory<byte>.Empty;
                }
            }
            _logger?.LogInformation($"{toWrite.Length} bytes left after filtering.");
            if (!toWrite.IsEmpty)
            {
                int written = await _client.WriteAsync(toWrite, cancellationToken);
                filterResultBuffer?.Dispose();

                if (0 >= written) { return new PipeWriteResult(Failed, written); }
                else { return new PipeWriteResult(Succeeded, written); }

            }
            else { return new PipeWriteResult(Succeeded, 0); }

        }

        /// <summary>
        /// Could return empty buffer.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="filters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Could return empty buffer.</returns>
        PipeFilterResult ExecuteFilter_BeforeWriting(IClient client, ReadOnlyMemory<byte> data, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = null;
            bool @continue = true;
            int time = 0;
            foreach (var filter in filters.Reverse())
            {
                try
                {
                    if (time > 0 && null == prevFilterMemory) { @continue = true; break; }
                    var result = filter.BeforeWriting(new PipeFilterContext(client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
                    time++;
                    prevFilterMemory?.Dispose();
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


        public void ApplyFilter(PipeFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if (object.ReferenceEquals(filter.Client, _client) && !_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }
    }
}
