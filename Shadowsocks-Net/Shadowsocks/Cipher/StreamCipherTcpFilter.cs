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

namespace Shadowsocks.Cipher
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;

    public class StreamCipherTcpFilter : PipeFilter
    {
        IShadowsocksStreamCipher _aead = null;
        public StreamCipherTcpFilter(IClient tcpClient, IShadowsocksStreamCipher cipher, ILogger logger = null)
               : base(tcpClient, 10, logger)
        {
            _aead = Throw.IfNull(() => cipher);
        }
        public override PipeFilterResult AfterReading(PipeFilterContext filterContext)
        {
            throw new NotImplementedException();
        }

        public override PipeFilterResult BeforeWriting(PipeFilterContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
