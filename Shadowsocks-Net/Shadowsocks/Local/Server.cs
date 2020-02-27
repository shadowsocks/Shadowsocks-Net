/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Net;
using Argument.Check;


namespace Shadowsocks.Local
{
    public sealed class Server
    {

        [JsonPropertyName("remarks")]
        public string Name { set; get; }

        [JsonPropertyName("server")]
        public string Address { set; get; }

        [JsonPropertyName("server_port")]
        public ushort Port { set; get; }

        [JsonPropertyName("password")]
        public string Password { set; get; }

        [JsonPropertyName("method")]
        public string Cipher { set; get; }

        [JsonPropertyName("obfs")]
        public string Obfuscator { set; get; }

        [JsonPropertyName("timeout")]
        public byte Timeout { set; get; }

        [JsonPropertyName("category")]
        public string Category { set; get; }


        static Dictionary<string, Type> cipherTypeCache = null;
        public Server()
        {

        }
        static Server()
        {
           
        }

        public async Task<IPEndPoint> GetIPEndPoint()
        {
            if(IPAddress.TryParse(this.Address,out IPAddress ip)){
                return new IPEndPoint(ip,this.Port);
            }
            else
            {
                try
                {
                    var ips = await Dns.GetHostAddressesAsync(this.Address);
                    if (ips.Length > 0)
                    {
                        return new IPEndPoint(ips[0], this.Port);
                    }
                }
                catch { }                                
            }
            return null;
        }



        public Cipher.IShadowsocksStreamCipher CreateCipher()
        {
            try
            {
                if (null == cipherTypeCache)
                {
                    cipherTypeCache = Helper.CipherLoader.LoadCiphers();
                }
                if (cipherTypeCache.ContainsKey(this.Cipher))//ToLower()
                {
                    return Activator.CreateInstance(cipherTypeCache[this.Cipher], this.Password) as Cipher.IShadowsocksStreamCipher;
                }
            }
            catch { }
            return null;
        }
    }
}
