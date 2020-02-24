/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks.Remote
{
    public class RemoteServerConfig
    {
        public int? Port { set; get; }
        public bool UseIPv6Address { set; get; }


        /// <summary>
        /// Applies to both UDP and TCP.
        /// </summary>
        public int? MaxNumClient { set; get; }

        public RemoteServerConfig()
        {
            Port ??= 8888;
            MaxNumClient ??= 100;
        }
    }
}
