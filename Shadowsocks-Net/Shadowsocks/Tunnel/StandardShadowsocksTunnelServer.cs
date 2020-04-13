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
using System.Net;
using Shadowsocks.Infrastructure.Sockets;

namespace Shadowsocks.Tunnel
{
    public sealed class StandardShadowsocksTunnelServer : ITunnelServer
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
