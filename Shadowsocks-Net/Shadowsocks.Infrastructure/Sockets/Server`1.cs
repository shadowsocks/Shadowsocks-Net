/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.Infrastructure.Sockets
{
    public abstract class Server<TClient> : IServer<TClient>
        where TClient : IClient
    {
        public virtual bool IsRunning { get; protected set; }

        public virtual IPEndPoint EndPoint { get; protected set; }


        public abstract Task<TClient> Accept();

        public abstract void Listen();

        public abstract void StopListen();
    }
}
