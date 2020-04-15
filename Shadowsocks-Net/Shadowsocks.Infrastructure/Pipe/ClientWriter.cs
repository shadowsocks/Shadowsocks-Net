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
    /// A client Writer with filter support.
    /// </summary>
    public sealed class ClientWriter : ClientReaderWriter<IClientWriterFilter>, IWriter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reverseFilterChain"></param>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public ClientWriter(IClient client, bool reverseFilterChain = false, int? bufferSize = 8192, ILogger logger = null)
        : base(client, reverseFilterChain, bufferSize, logger)
        {
        }

        /// <summary>
        /// Write to the client.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<WriteResult> Write(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
        {
            if (data.IsEmpty)
            {
                _logger?.LogWarning($"PipeWriter Try to write empty memory. [{Client.EndPoint.ToString()}].");
                return new WriteResult(Failed, 0);
            }

            ReadOnlyMemory<byte> toWrite = data;
            SmartBuffer filterResultBuffer = null;
            if (FilterEnabled && FilterChain.Count > 0)
            {
                var result = ExecuteFilter(data, cancellationToken);
                if (!result.Continue)
                {
                    result.Buffer?.Dispose();
                    _logger?.LogInformation($"Pipe broke by filter [{Client.EndPoint.ToString()}].");
                    return new WriteResult(BrokeByFilter, 0);
                }
                else
                {
                    filterResultBuffer = result.Buffer;
                    toWrite = null != filterResultBuffer ? filterResultBuffer.SignificantMemory : ReadOnlyMemory<byte>.Empty;
                }
            }
            _logger?.LogInformation($"{toWrite.Length} bytes left after [OnWriting] filtering.");
            if (!toWrite.IsEmpty)
            {
                int written = await Client.WriteAsync(toWrite, cancellationToken);
                filterResultBuffer?.Dispose();

                if (0 >= written) { return new WriteResult(Failed, written); }
                else { return new WriteResult(Succeeded, written); }

            }
            else { return new WriteResult(Succeeded, 0); }

        }


        /// <summary>
        /// 
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
                    var result = filter.OnWriting(new ClientFilterContext(Client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
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
                    _logger?.LogError(ex, $"ExecuteFilter_OnWriting [{Client.EndPoint.ToString()}].");
                }
            }
            return new ClientFilterResult(Client, prevFilterMemory, @continue);
        }



    }
}
