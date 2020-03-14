/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks.Http
{
    using Infrastructure;
    using Infrastructure.Sockets;

    public class HttpProxyServerConfig : ServerConfig
    {
        //[JsonPropertyName("http_port")]
        public int Port { set; get; }

        //[JsonPropertyName("use_ipv6")]
        public bool UseIPv6Address { set; get; }


        //[JsonPropertyName("use_loopback")]
        public bool UseLoopbackAddress { set; get; }
        public HttpProxyServerConfig()
        {
            if (0 == Port) { Port = 8080; }
        }


        public IPEndPoint GetBindPoint()
        {
            if (UseLoopbackAddress)
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Loopback : IPAddress.Loopback, Port);
            }
            else
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Any : IPAddress.Any, Port);
            }
        }
    }
}
