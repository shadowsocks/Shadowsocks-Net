/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks.Infrastructure.Sockets
{
    public class ServerConfig
    {
        public int Port { set; get; }

        public IPEndPoint IPEndPoint { set; get; }//TODO bind failed.
        public bool UseIPv6Address { set; get; }
        public bool UseLoopbackAddress { set; get; }



        public ServerConfig()
        {
        }
    }
}
