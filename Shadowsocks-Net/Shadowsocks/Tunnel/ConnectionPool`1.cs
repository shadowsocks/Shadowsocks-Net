/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

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

namespace Shadowsocks.Tunnel
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;
    public class ConnectionPool<TClient>
         where TClient : IClient
    {
    }
}
