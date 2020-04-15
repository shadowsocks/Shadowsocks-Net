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

    public class UdpCipherFilter : ClientFilter
    {
        IShadowsocksStreamCipher _cipher = null;
        public UdpCipherFilter(IShadowsocksStreamCipher cipher, ILogger logger = null)
               : base(ClientFilterCategory.Cipher, 0, logger)
        {
            _cipher = Throw.IfNull(() => cipher);
        }
        public override ClientFilterResult OnReading(ClientFilterContext ctx)
        {
            if (!ctx.Memory.IsEmpty)
            {
                var bufferPlain = _cipher.DecryptUdp(ctx.Memory);
                if (null != bufferPlain && bufferPlain.SignificantLength > 0)
                {
                    return new ClientFilterResult(this.Client, bufferPlain, true);
                }
                else { _logger?.LogError($"CipherUdpFilter OnReading no plain data."); }
            }
            else { _logger?.LogError($"CipherUdpFilter OnReading filterContext.Memory.IsEmpty"); }

            return new ClientFilterResult(this.Client, null, false);
        }

        public override ClientFilterResult OnWriting(ClientFilterContext ctx)
        {
            if (!ctx.Memory.IsEmpty)
            {
                var bufferCipher = _cipher.EncryptUdp(ctx.Memory);
                return new ClientFilterResult(this.Client, bufferCipher, true);
            }
            else { _logger?.LogError($"CipherUdpFilter OnWriting filterContext.Memory.IsEmpty"); }

            return new ClientFilterResult(this.Client, null, false);
        }
    }
}
