/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Buffers;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// DNS resolution & caching service.
    /// </summary>
    public class DnsCache
    {
        ILogger _logger = null;
        LruCache<IPAddress[]> _cache = null;

        readonly TimeSpan _expiretime = TimeSpan.FromMinutes(30);

        public DnsCache(ILogger logger = null)
        {
            _cache = new LruCache<IPAddress[]>(TimeSpan.FromMinutes(10));
            _logger = logger;
        }

        public async Task<IPAddress[]> ResolveHost(string host)
        {
           
            var cache = _cache.Get(host);
            if (null != cache) { return await Task.FromResult(cache); }
            try
            {
                _logger?.LogInformation($"DnsCache resolving [{host}]...");
                var entry = await Dns.GetHostEntryAsync(host);
                if (null != entry.AddressList && entry.AddressList.Length > 0)
                {
                    _logger?.LogInformation($"DnsCache resolved [{host}] = [{entry.AddressList[0].ToString()}]");
                    _cache.Set(host, entry.AddressList, _expiretime);
                    return entry.AddressList;
                }
            }
            catch (SocketException se)
            {
                _logger?.LogWarning($"DnsCache resolve hostname failed:[{host}]. {se.SocketErrorCode}, {se.Message}.");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"DnsCache resolve hostname failed:[{host}]. {ex.Message}.");
            }

            return null;
        }
    }
}
