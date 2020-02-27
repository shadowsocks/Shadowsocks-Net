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
using System.Net.Sockets;

namespace Shadowsocks.Local
{
    public class LocalServerConfig
    {
        //[JsonPropertyName("socks5_port")]
        public int Socks5Port { set; get; }


        //[JsonPropertyName("http_port")]
        public int HttpPort { set; get; }


        //[JsonPropertyName("use_ipv6")]
        public bool UseIPv6Address { set; get; }


        //[JsonPropertyName("use_loopback")]
        public bool UseLoopbackAddress { set; get; }




        public LocalServerConfig()
        {
            if (0 == Socks5Port) { Socks5Port = 1080; }
            if (0 == HttpPort) { HttpPort = 8080; }
        }


        public IPEndPoint GetSocks5IPEndPoint()
        {
            if (UseLoopbackAddress)
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Loopback : IPAddress.Loopback, (int)Socks5Port);
            }
            else
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Any : IPAddress.Any, (int)Socks5Port);
            }
        }

        public IPEndPoint GetHttpIPEndPoint()
        {
            if (UseLoopbackAddress)
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Loopback : IPAddress.Loopback, (int)HttpPort);
            }
            else
            {
                return new IPEndPoint((UseIPv6Address && Socket.OSSupportsIPv6) ? IPAddress.IPv6Any : IPAddress.Any, (int)HttpPort);
            }
        }
    }
}
