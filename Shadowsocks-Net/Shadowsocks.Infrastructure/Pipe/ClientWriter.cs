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
    using static ClientReadWriteResult;

    /// <summary>
    /// A client writer with filter support.
    /// </summary>
    public class ClientWriter : IClientWriter
    {
        /// <summary>
        /// The client.
        /// </summary>
        public IClient Client { get; private set; }

        /// <summary>
        /// Applied filters.
        /// </summary>
        public IReadOnlyCollection<IClientWriterFilter> Filters => _filters;

        SortedSet<IClientWriterFilter> _filters = null;
        int _bufferSize = 8192;
        ILogger _logger = null;


        /// <summary>
        /// Create a ClientWriter with a client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public ClientWriter(IClient client, int? bufferSize = 8192, ILogger logger = null)
        {
            Client = Throw.IfNull(() => client);
            _filters = new SortedSet<IClientWriterFilter>();
            _bufferSize = bufferSize ?? 8192;

            _logger = logger;
        }


        /// <summary>
        /// Create a ClientWriter with a client and it's filters.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="filters"></param>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public ClientWriter(IClient client, IEnumerable<IClientWriterFilter> filters, int? bufferSize = 8192, ILogger logger = null)
          : this(client, bufferSize, logger)
        {

            Throw.IfNull(() => filters);

            foreach (var f in filters)
            {
                this.ApplyFilter(f);
            }
        }

        /// <summary>
        /// Write to the client.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<ClientWriteResult> Write(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
        {
            if (data.IsEmpty)
            {
                _logger?.LogWarning($"PipeWriter Try to write empty memory. [{Client.EndPoint.ToString()}].");
                return new ClientWriteResult(Failed, 0);
            }

            ReadOnlyMemory<byte> toWrite = data;
            SmartBuffer filterResultBuffer = null;
            if (_filters.Count > 0)
            {
                var result = ExecuteFilter_BeforeWriting(Client, data, _filters, cancellationToken);
                if (!result.Continue)
                {
                    result.Buffer?.Dispose();
                    _logger?.LogInformation($"Pipe broke by filter [{Client.EndPoint.ToString()}].");
                    return new ClientWriteResult(BrokeByFilter, 0);
                }
                else
                {
                    filterResultBuffer = result.Buffer;
                    toWrite = null != filterResultBuffer ? filterResultBuffer.SignificantMemory : ReadOnlyMemory<byte>.Empty;
                }
            }
            _logger?.LogInformation($"{toWrite.Length} bytes left after [BeforeWriting] filtering.");
            if (!toWrite.IsEmpty)
            {
                int written = await Client.WriteAsync(toWrite, cancellationToken);
                filterResultBuffer?.Dispose();

                if (0 >= written) { return new ClientWriteResult(Failed, written); }
                else { return new ClientWriteResult(Succeeded, written); }

            }
            else { return new ClientWriteResult(Succeeded, 0); }

        }


        /// <summary>
        /// Apply a <see cref="IClientWriterFilter"/>  filter.
        /// </summary>
        /// <param name="filter"></param>
        public void ApplyFilter(IClientWriterFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if (object.ReferenceEquals(filter.Client, Client) && !_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }

        /// <summary>
        /// Execute filters.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="filters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ClientFilterResult ExecuteFilter_BeforeWriting(IClient client, ReadOnlyMemory<byte> data, SortedSet<IClientWriterFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = null;
            bool @continue = true;
            int time = 0;
            foreach (var filter in filters.Reverse())
            {
                try
                {
                    if (time > 0 && null == prevFilterMemory) { @continue = true; break; }
                    var result = filter.BeforeWriting(new ClientFilterContext(client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
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
            return new ClientFilterResult(client, prevFilterMemory, @continue);
        }



    }
}
