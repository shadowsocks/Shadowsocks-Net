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
    public sealed class ClientReader : IClientReader
    {
        public IClient Client { get; private set; }

        public IReadOnlyCollection<IClientReaderFilter> Filters => _filters;



        SortedSet<IClientReaderFilter> _filters = null;
        int _bufferSize = 8192;
        ILogger _logger = null;


        public ClientReader(IClient client, int? bufferSize = 8192, ILogger logger = null)
        {
            Client = Throw.IfNull(() => client);
            _filters = new SortedSet<IClientReaderFilter>();
            _bufferSize = bufferSize ?? 8192;

            _logger = logger;
        }

        public ClientReader(IClient client, IEnumerable<IClientReaderFilter> filters, int? bufferSize = 8192, ILogger logger = null)
            :this(client, bufferSize, logger)
        {
            Throw.IfNull(() => filters);

            foreach (var f in filters)
            {
                this.ApplyFilter(f);
            }
        }

        public async ValueTask<ClientReadResult> Read(CancellationToken cancellationToken)
        {
            var received = SmartBuffer.Rent(_bufferSize);
            received.SignificantLength = await Client.ReadAsync(received.Memory, cancellationToken);
            _logger?.LogInformation($"PipeReader Received {received.SignificantLength} bytes from [{Client.EndPoint.ToString()}].");

            if (0 >= received.SignificantLength)
            {
                received.Dispose();
                return new ClientReadResult(Failed, null, received.SignificantLength);
            }

            if (_filters.Count > 0)
            {
                var result = ExecuteFilter_AfterReading(Client, received.SignificantMemory, _filters, cancellationToken);
                received.Dispose();
                received = result.Buffer;
                if (!result.Continue)
                {
                    received?.Dispose();
                    return new ClientReadResult(BrokeByFilter, null, 0);
                }
            }
            return new ClientReadResult(Succeeded, received, null != received ? received.SignificantLength : 0);

        }
        public void ApplyFilter(IClientReaderFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);

            if (object.ReferenceEquals(filter.Client, Client) && !_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }

        /// <summary>
        /// Could return empty buffer.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="filters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Could return empty buffer.</returns>
        ClientFilterResult ExecuteFilter_AfterReading(IClient client, ReadOnlyMemory<byte> data, SortedSet<IClientReaderFilter> filters, CancellationToken cancellationToken)
        {
            SmartBuffer prevFilterMemory = null;
            bool @continue = true;
            int time = 0;
            foreach (var filter in filters)
            {
                try
                {
                    if (time > 0 && null == prevFilterMemory) { @continue = true; break; }
                    var result = filter.AfterReading(new ClientFilterContext(client, null == prevFilterMemory ? data : prevFilterMemory.SignificantMemory));
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
            return new ClientFilterResult(client, prevFilterMemory, @continue);
        }




    }
}
