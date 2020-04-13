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
using Shadowsocks.Infrastructure.Sockets;

namespace Shadowsocks.Tunnel
{
    public sealed class StandardShadowsocksTunnelClient : ITunnel
    {
        //ip?
        //Server
        public StandardShadowsocksTunnelClient()
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
