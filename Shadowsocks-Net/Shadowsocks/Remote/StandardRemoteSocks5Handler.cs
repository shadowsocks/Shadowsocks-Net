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


namespace Shadowsocks.Remote
{
    using Infrastructure;
    using Infrastructure.Sockets;

    //pipe broken -> remove

    class StandardRemoteSocks5Handler : ISocks5Handler
    {


        public async Task HandleTcp(IClient tcpClient, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public async Task HandleUdp(IClient udpClient, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
