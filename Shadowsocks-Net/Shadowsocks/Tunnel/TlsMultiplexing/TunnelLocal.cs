/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Argument.Check;
using Shadowsocks.Infrastructure.Sockets;

namespace Shadowsocks.Tunnel.TlsMultiplexing
{
    class TunnelLocal : ITunnelLocal
    {
        public Task<IClient> ConnectTcp()
        {
            throw new NotImplementedException();
        }

        public Task<IClient> ConnectUdp()
        {
            throw new NotImplementedException();
        }
    }
}
