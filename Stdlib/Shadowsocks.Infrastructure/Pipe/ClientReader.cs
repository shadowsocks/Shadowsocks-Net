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
    using static ReadWriteResult;

    /// <summary>
    /// A client Reader with filter support.
    /// </summary>
    public sealed class ClientReader : ClientReaderWriter<IClientReaderFilter>, IReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reverseFilterChain"></param>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public ClientReader(IClient client, bool reverseFilterChain = false, int? bufferSize = 8192, ILogger logger = null)
            : base(client, reverseFilterChain, bufferSize, logger)
        {
        }

        /// <summary>
        /// Read the client.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<ReadResult> Read(CancellationToken cancellationToken)
        {
            var received = SmartBuffer.Rent(_bufferSize);
            received.SignificantLength = await Client.ReadAsync(received.Memory, cancellationToken);
            _logger?.LogInformation($"PipeReader Received {received.SignificantLength} bytes from [{Client.EndPoint.ToString()}].");

            if (0 >= received.SignificantLength)
            {
                received.Dispose();
                return new ReadResult(Failed, null, received.SignificantLength);
            }

            if (FilterEnabled && FilterChain.Count > 0)
            {
                var result = ExecuteFilter(received.SignificantMemory, cancellationToken);
                received.Dispose();
                received = result.Buffer;
                if (!result.Continue)
                {
                    received?.Dispose();
                    return new ReadResult(BrokeByFilter, null, 0);
                }
            }
            int read = null != received ? received.SignificantLength : 0;
            _logger?.LogInformation($"{read} bytes left after [OnReading] filtering.");

            return new ReadResult(Succeeded, received, read);
        }



        /// <summary>
        /// Execute filters.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected ClientFilterResult ExecuteFilter(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = null;
            bool @continue = true;
            int time = 0;

            foreach (var filter in FilterChain)
            {
                try
                {
                    if (time > 0 && null == prevFilterMemory) { @continue = true; break; }
                    var result = filter.OnReading(new ClientFilterContext(Client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
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
                    _logger?.LogError(ex, $"PipeReader ExecuteFilter_OnReading [{Client.EndPoint.ToString()}].");
                }
            }
            return new ClientFilterResult(Client, prevFilterMemory, @continue);
        }




    }
}
