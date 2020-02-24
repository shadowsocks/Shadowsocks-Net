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

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// Represent the client in a client-server socket program.
    /// </summary>
    public partial interface IClient : IPeer
    {        
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
        ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    }

    public partial interface IClient : IPeer
    {
        IPEndPoint LocalEndPoint { get; }
        void Close();
        event EventHandler<ClientEventArgs> Closing;
    }


    ///// <summary>
    ///// Use SocketAsyncEventArgs.
    ///// </summary>
    //public interface IClientEAP : IPeer
    //{
    //    void PostRead(Memory<byte> buffer);
    //    void PostWrite(ReadOnlyMemory<byte> buffer);

    //    event EventHandler<ClientReadEventArgs> OnRead;
    //    event EventHandler<ClientWriteEventArgs> OnWrite;
    //}


}
