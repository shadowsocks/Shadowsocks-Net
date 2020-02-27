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
        public IPEndPoint BindPoint { set; get; }//TODO bind failed.

        public int? MaxNumClient { set; get; } // 100;


        public ServerConfig()
        {
            MaxNumClient ??= Defaults.MaxNumClient;
        }
    }
}
