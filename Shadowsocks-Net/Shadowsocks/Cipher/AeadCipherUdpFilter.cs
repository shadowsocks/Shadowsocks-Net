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

    public class AeadCipherUdpFilter : PipeFilter
    {
        IShadowsocksAeadCipher _aead = null;
        public AeadCipherUdpFilter(IClient udpClient, IShadowsocksAeadCipher cipher, ILogger logger = null)
               : base(udpClient, 10, logger)
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
