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
    using Infrastructure.Pipe;


#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class StandardRemoteSocks5Handler : ISocks5Handler
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        ILogger _logger = null;

        List<DefaultPipe> _pipes = null;
        object _pipesReadWriteLock = new object();

        Type _cipherType = null;
        string _cipherPassword = null;
        public StandardRemoteSocks5Handler(Type cipherType, string cipherPassword, ILogger logger = null)
        {
            _cipherType = Throw.IfNull(() => cipherType);
            _cipherPassword = Throw.IfNullOrEmpty(() => cipherPassword);
            _logger = logger;
        }
        ~StandardRemoteSocks5Handler()
        {
            Cleanup();
        }


        public async Task HandleTcp(IClient tcpClient, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public async Task HandleUdp(IClient udpClient, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        void Cleanup()
        {
            foreach (var p in this._pipes)
            {
                p.UnPipe();
                p.ClientA.Close();
                p.ClientB.Close();
            }
            lock (_pipesReadWriteLock)
            {
                this._pipes.Clear();
            }

        }
        public void Dispose()
        {
            Cleanup();
        }
    }
}
