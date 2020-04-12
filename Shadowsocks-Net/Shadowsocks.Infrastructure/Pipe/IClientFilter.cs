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
using Argument.Check;
using Microsoft.Extensions.Logging;

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    public interface IClientFilter : IClientReaderFilter, IClientWriterFilter, IClientObject
    {
        new IClient Client { set; get; }
    }
}

