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

namespace Shadowsocks.Tunnel
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;

    public abstract class TunnelClient : IClient
    {
        public virtual IPEndPoint LocalEndPoint { protected set; get; }
        public virtual IPEndPoint EndPoint { protected set; get; }

        public event EventHandler<ClientEventArgs> Closing;

        protected ILogger _logger = null;

        protected TunnelClient(IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ILogger logger = null)
        {
            EndPoint = remoteEndPoint;
            LocalEndPoint = localEndPoint;

            _logger = logger;
        }

        public abstract void Close();
        public abstract ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
        public abstract ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
