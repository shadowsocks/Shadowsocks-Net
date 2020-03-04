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
        public UdpCipherFilter(IClient tcpClient, IShadowsocksStreamCipher cipher, ILogger logger = null)
               : base(tcpClient, ClientFilterCategory.Cipher, 0)
        {
            _cipher = Throw.IfNull(() => cipher);
            _logger = logger;
        }
        public override ClientFilterResult AfterReading(ClientFilterContext ctx)
        {          
            if (!ctx.Memory.IsEmpty)
            {
                var bufferPlain = _cipher.DecryptUdp(ctx.Memory);
                if (null != bufferPlain && bufferPlain.SignificantLength > 0)
                {
                    return new ClientFilterResult(this.Client, bufferPlain, true);                    
                }
                else { _logger?.LogError($"CipherUdpFilter AfterReading no plain data."); }
            }
            else { _logger?.LogError($"CipherUdpFilter AfterReading filterContext.Memory.IsEmpty"); }

            return new ClientFilterResult(this.Client, null, false);
        }

        public override ClientFilterResult BeforeWriting(ClientFilterContext ctx)
        {
            if (!ctx.Memory.IsEmpty)
            {
                var bufferCipher = _cipher.EncryptUdp(ctx.Memory);
                return new ClientFilterResult(this.Client, bufferCipher, true);
            }
            else { _logger?.LogError($"CipherUdpFilter BeforeWriting filterContext.Memory.IsEmpty"); }

            return  new ClientFilterResult(this.Client, null, false);
        }
    }
}
