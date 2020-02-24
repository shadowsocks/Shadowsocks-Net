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

    public class AeadCipherTcpFilter : PipeFilter
    {
        IShadowsocksAeadCipher _aead = null;
        public AeadCipherTcpFilter(IClient tcpClient, IShadowsocksAeadCipher cipher, ILogger logger = null)
               : base(tcpClient, 10, logger)
        {
            _aead = Throw.IfNull(() => cipher);
        }
        public override PipeFilterResult AfterReading(PipeFilterContext filterContext)
        {
            PipeFilterResult r = new PipeFilterResult(this.Client, null, false);
            if (null != _aead)
            {
                if (!filterContext.Memory.IsEmpty)
                {
                    var cipher = _aead.DecryptTcp(filterContext.Memory.Slice(0, filterContext.MemoryLength));
                    if (null != cipher && cipher.SignificantLength > 0)
                    {
                        r = new PipeFilterResult(this.Client, cipher, true);
                    }
                    else
                    {
                        _logger?.LogError($"AeadCipherTcpFilter AfterReading no cipher data.");
                    }
                }
                else
                {
                    _logger?.LogError($"AeadCipherTcpFilter AfterReading filterContext.Memory.IsEmpty");
                }
            }
            return r;
        }

        public override PipeFilterResult BeforeWriting(PipeFilterContext filterContext)
        {
            PipeFilterResult r = new PipeFilterResult(this.Client, null, false);
            if (null != _aead)
            {
                if (!filterContext.Memory.IsEmpty)
                {
                    var cipher = _aead.EncryptTcp(filterContext.Memory.Slice(0, filterContext.MemoryLength));
                    r = new PipeFilterResult(this.Client, cipher, true);
                }
                else
                {
                    _logger?.LogError($"AeadCipherTcpFilter BeforeWriting filterContext.Memory.IsEmpty");
                }
            }
            return r;
        }
    }
}
