/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Shadowsocks.Infrastructure.Sockets
{
    public class TcpServerConfig : ServerConfig
    {

        public bool KeepAlive { set; get; }
        public int KeepAliveTime { set; get; }
        public int KeepAliveInterval { set; get; }


        public TcpServerConfig()
        {
            //Port = 8888;
            //KeepAlive = true;
            KeepAliveTime = 10 * 1000;
            KeepAliveInterval = 10 * 1000;

        }
    }
}
