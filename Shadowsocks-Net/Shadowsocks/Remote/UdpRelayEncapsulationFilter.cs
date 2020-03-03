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
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Remote
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;
    /*
              
                # shadowsocks UDP Request (before encrypted)
                # +------+----------+----------+----------+
                # | ATYP | DST.ADDR | DST.PORT |   DATA   |
                # +------+----------+----------+----------+
                # |  1   | Variable |    2     | Variable |
                # +------+----------+----------+----------+

                # shadowsocks UDP Response (before encrypted)
                # +------+----------+----------+----------+
                # | ATYP | DST.ADDR | DST.PORT |   DATA   |
                # +------+----------+----------+----------+
                # |  1   | Variable |    2     | Variable |
                # +------+----------+----------+----------+    
     */
    /// <summary>
    /// Applies to UDP relay client.
    /// </summary>
    class UdpRelayEncapsulationFilter : PipeFilter
    {

        public UdpRelayEncapsulationFilter(IClient udpClient, ILogger logger = null)
               : base(udpClient, PipeFilterCategory.Encapsulation, 0)
        {
            _logger = logger;
        }
        public override PipeFilterResult BeforeWriting(PipeFilterContext ctx)
        {
            SmartBuffer toTarget = SmartBuffer.Rent(ctx.Memory.Length);
            if (ShadowsocksAddress.TryResolveLength(ctx.Memory, out int targetAddrLen))
            {
                ctx.Memory.Slice(targetAddrLen).CopyTo(toTarget.Memory);
                toTarget.SignificantLength = ctx.Memory.Length - targetAddrLen;
                return new PipeFilterResult(ctx.Client, toTarget, true);
            }
            else
            {
                return new PipeFilterResult(ctx.Client, toTarget, false);
            }
        }

        public override PipeFilterResult AfterReading(PipeFilterContext ctx)
        {
            SmartBuffer toSsLocal = SmartBuffer.Rent(1500);//TODO what if exceeds 1500? fragments or not?

            if (ShadowsocksAddress.TrySerailizeTo(
                               (byte)(AddressFamily.InterNetworkV6 == ctx.Client.EndPoint.AddressFamily ? 0x4 : 0x1),
                               ctx.Client.EndPoint.Address.GetAddressBytes(),
                               (ushort)ctx.Client.EndPoint.Port,
                               toSsLocal.Memory,
                               out int written))
            {
                toSsLocal.SignificantLength = written;
                int payloadToCopy = Math.Min(toSsLocal.FreeSpace, ctx.Memory.Length);
                ctx.Memory.Slice(0, payloadToCopy).CopyTo(toSsLocal.FreeMemory);
                toSsLocal.SignificantLength += payloadToCopy;

                return new PipeFilterResult(ctx.Client, toSsLocal, true); ;
            }

            return new PipeFilterResult(ctx.Client, null, false); ;
        }


    }
}
