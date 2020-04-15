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
using System.IO;
using Microsoft.Extensions.Logging;
using Argument.Check;
using System.Text;

namespace Shadowsocks.Tunnel.ClassicalShadowsocks
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;

    public sealed class TunnelLocal : ITunnelLocal
    {
        //ip?
        //Server
        public TunnelLocal()
        {

        }

        public Task<IClient> ConnectTcp()
        {
            //TcpClient1.ConnectAsync()
            throw new NotImplementedException();
        }

        public Task<IClient> ConnectUdp()
        {
            throw new NotImplementedException();
        }
    }
}
