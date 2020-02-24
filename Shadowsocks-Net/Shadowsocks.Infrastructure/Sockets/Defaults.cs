/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Sockets
{
    public static class Defaults
    {
        public static readonly int MaxNumClient = 50;// 100;

        public static readonly int SendBufferSize = 8192;
        public static readonly int ReceiveBufferSize = 8192;

        public static readonly int SendTimeout = 5000;
        public static readonly int ReceiveTimeout = 5000;

        //public static readonly byte Ttl=
    }
}
