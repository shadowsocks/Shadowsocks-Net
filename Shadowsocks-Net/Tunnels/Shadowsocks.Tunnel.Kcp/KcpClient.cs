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

namespace Shadowsocks.Tunnel.Kcp
{
    using Infrastructure;
    using Infrastructure.Sockets;
    
    /// <summary>
    /// Clients use KCP protocol.
    /// </summary>
    public class KcpClient : IClient
    {
        public IPEndPoint LocalEndPoint => throw new NotImplementedException();

        public IPEndPoint EndPoint => throw new NotImplementedException();

        public event EventHandler<ClientEventArgs> Closing;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected virtual void FireClosing()
        {
            try
            {
                if (null != Closing)
                {
                    Closing(this, new ClientEventArgs(this));
                }
            }
            catch// (Exception ex)
            {
                //_logger?.LogError(ex, "ClientBase error fire Closing.");
            }
        }
    }
}
