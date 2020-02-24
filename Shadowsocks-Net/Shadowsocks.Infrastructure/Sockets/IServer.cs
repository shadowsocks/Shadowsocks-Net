/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// Represent the server part of a client-server socket program.
    /// </summary>
    public interface IServer : IPeer
    {
        void Listen();
        void StopListen();
    }

    public interface IServer<TClient> : IServer
        where TClient : IClient
    {
        Task<TClient> Accept();       
    }

}
