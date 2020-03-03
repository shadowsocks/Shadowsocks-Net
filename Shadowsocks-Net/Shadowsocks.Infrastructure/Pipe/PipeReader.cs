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
    public sealed class PipeReader
    {
        IClient _client = null;
        SortedSet<PipeFilter> _filters = null;
        int _bufferSize = 8192;
        ILogger _logger = null;


        public PipeReader(IClient client, int? bufferSize = 8192, ILogger logger = null)
        {
            _client = Throw.IfNull(() => client);
            _filters = new SortedSet<PipeFilter>();
            _bufferSize = bufferSize ?? 8192;

            _logger = logger;
        }

        public async ValueTask<PipeReadResult> Read(CancellationToken cancellationToken)
        {
            var received = SmartBuffer.Rent(_bufferSize);
            received.SignificantLength = await _client.ReadAsync(received.Memory, cancellationToken);
            _logger?.LogInformation($"PipeReader Received {received.SignificantLength} bytes from [{_client.EndPoint.ToString()}].");

            if (0 >= received.SignificantLength)
            {                
                received.Dispose();
                return new PipeReadResult(Failed, null, received.SignificantLength);
            }

            if (_filters.Count > 0)
            {
                var result = ExecuteFilter_AfterReading(_client, received.SignificantMemory, _filters, cancellationToken);
                received.Dispose();
                received = result.Buffer;
                if (!result.Continue)
                {
                    received?.Dispose();
                    return new PipeReadResult(BrokeByFilter, null, 0);
                }
            }
            return new PipeReadResult(Succeeded, received, null != received ? received.SignificantLength : 0);

        }


        /// <summary>
        /// Could return empty buffer.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="filters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Could return empty buffer.</returns>
        PipeFilterResult ExecuteFilter_AfterReading(IClient client, ReadOnlyMemory<byte> data, SortedSet<PipeFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = null;
            bool @continue = true;
            int time = 0;
            foreach (var filter in filters)
            {
                try
                {
                    if (time > 0 && null == prevFilterMemory) { @continue = true; break; }
                    var result = filter.AfterReading(new PipeFilterContext(client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
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
                    _logger?.LogError(ex, $"PipeReader ExecuteFilter_AfterReading [{client.EndPoint.ToString()}].");
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
