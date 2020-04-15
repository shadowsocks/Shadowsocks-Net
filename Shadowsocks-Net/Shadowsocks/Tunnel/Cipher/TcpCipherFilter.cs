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

    public class TcpCipherFilter : ClientFilter
    {
        IShadowsocksStreamCipher _cipher = null;
        public TcpCipherFilter(IShadowsocksStreamCipher cipher, ILogger logger = null)
               : base(ClientFilterCategory.Cipher, 0, logger)
        {
            _cipher = Throw.IfNull(() => cipher);            
        }
        public override ClientFilterResult OnReading(ClientFilterContext ctx)
        {
            SmartBuffer bufferPlain = null;
            if (null != _cipher)
            {
                if (!ctx.Memory.IsEmpty)//TODO 1
                {
                    bufferPlain = _cipher.DecryptTcp(ctx.Memory);
                }
                else
                {
                    _logger?.LogError($"AeadCipherTcpFilter OnReading filterContext.Memory.IsEmpty");
                }
            }
            return new ClientFilterResult(this.Client, bufferPlain, true);//TODO 2           
        }

        public override ClientFilterResult OnWriting(ClientFilterContext ctx)
        {
            ClientFilterResult r = new ClientFilterResult(this.Client, null, false);
            if (null != _cipher)
            {
                if (!ctx.Memory.IsEmpty)
                {
                    var bufferCipher = _cipher.EncryptTcp(ctx.Memory);
                    r = new ClientFilterResult(this.Client, bufferCipher, true);
                }
                else
                {
                    _logger?.LogError($"AeadCipherTcpFilter OnWriting filterContext.Memory.IsEmpty");
                }
            }
            return r;
        }
    }
}
