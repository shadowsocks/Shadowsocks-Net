/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Shadowsocks.Remote
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using System.Net.Sockets;

    public class RemoteServerConfig
    {
        [JsonPropertyName("server_host")]
        public string Address { set; get; }

        [JsonPropertyName("server_port")]
        public ushort Port { set; get; }

        [JsonPropertyName("use_ipv6")]
        public bool UseIPv6Address { set; get; }

        [JsonPropertyName("timeout")]
        public byte Timeout { set; get; }

        [JsonPropertyName("password")]
        public string Password { set; get; }

        [JsonPropertyName("method")]
        public string Cipher { set; get; }

        /// <summary>
        /// Applies to both UDP and TCP.
        /// </summary>
        [JsonIgnore]
        public int? MaxNumClient { set; get; }


        Dictionary<string, Type> _cipherTypeCache = null;


        public RemoteServerConfig()
        {
            MaxNumClient ??= Defaults.MaxNumClient;
        }


        public IPEndPoint GetIPEndPoint()
        {
            if (!string.IsNullOrEmpty(this.Address))
            {
                if (IPAddress.TryParse(this.Address, out IPAddress ip))
                {
                    return new IPEndPoint(ip, this.Port);
                }
                return new IPEndPoint(IPAddress.Any, this.Port);
            }
            else
            {
                if (UseIPv6Address && Socket.OSSupportsIPv6)
                {
                    return new IPEndPoint(IPAddress.IPv6Any, this.Port);
                }
                else
                {
                    return new IPEndPoint(IPAddress.Any, this.Port);
                }
            }
        }

        public Cipher.IShadowsocksStreamCipher CreateCipher(ILogger logger = null)
        {
            try
            {
                if (null == _cipherTypeCache)
                {
                    _cipherTypeCache = Helper.CipherLoader.LoadCiphers();
                }

                if (_cipherTypeCache.ContainsKey(this.Cipher))//ToLower()
                {
                    return Activator.CreateInstance(_cipherTypeCache[this.Cipher], this.Password, null) as Cipher.IShadowsocksStreamCipher;
                }
            }
            catch (Exception ex) { Console.WriteLine($"{ex.Message}. {ex.StackTrace}"); }
            return null;
        }
    }
}
