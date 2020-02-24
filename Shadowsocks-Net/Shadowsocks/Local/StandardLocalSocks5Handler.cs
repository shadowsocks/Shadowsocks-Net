/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Argument.Check;


namespace Shadowsocks.Local
{
    using Infrastructure;
    using Infrastructure.Sockets;

    class StandardLocalSocks5Handler : ISocks5Handler
    {
        //accept->add
        //error->remove
        //expire->remove
        ClientCollection<TcpClient1> _clients = new ClientCollection<TcpClient1>();
        //TcpServer tcpServer

        //tcp lost->remove udp


        //pipe broken -> remove
        public void HandleTcp(IClient tcpClient)
        {
            throw new NotImplementedException();
        }

        public void HandelUdp(IClient udpClient)
        {
            throw new NotImplementedException();
        }
    }
}
