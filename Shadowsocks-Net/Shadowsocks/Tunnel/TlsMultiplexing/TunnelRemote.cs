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
    class TunnelRemote : ITunnelRemote
    {
        public IPEndPoint EndPoint => throw new NotImplementedException();

        public Task<IClient> AcceptTcp()
        {
            throw new NotImplementedException();
        }

        public Task<IClient> AcceptUdp()
        {
            throw new NotImplementedException();
        }

        public void Listen()
        {
            throw new NotImplementedException();
        }

        public void StopListen()
        {
            throw new NotImplementedException();
        }
    }
}
