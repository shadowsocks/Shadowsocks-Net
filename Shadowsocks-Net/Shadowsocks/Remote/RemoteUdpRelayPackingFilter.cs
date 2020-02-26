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
      +----+------+------+----------+----------+----------+
      |RSV | FRAG | ATYP | DST.ADDR | DST.PORT |   DATA   |
      +----+------+------+----------+----------+----------+
      | 2  |  1   |  1   | Variable |    2     | Variable |
      +----+------+------+----------+----------+----------+

     The fields in the UDP request header are:

          o  RSV  Reserved X'0000'
          o  FRAG    Current fragment number
          o  ATYP    address type of following addresses:
             o  IP V4 address: X'01'
             o  DOMAINNAME: X'03'
             o  IP V6 address: X'04'
          o  DST.ADDR       desired destination address
          o  DST.PORT       desired destination port
          o  DATA     user data     
     
     
     
     
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
    class RemoteUdpRelayPackingFilter : PipeFilter
    {

        public RemoteUdpRelayPackingFilter(IClient udpClient, ILogger logger = null)
               : base(udpClient, 20, logger)
        {

        }

        public override PipeFilterResult AfterReading(PipeFilterContext ctx)
        {
            SmartBuffer toLocal = SmartBuffer.Rent(1500);//TODO what if exceeds 1500? fragments or not?


            if (ShadowsocksAddress.TrySerailizeTo(
                               (byte)(AddressFamily.InterNetworkV6 == ctx.Client.EndPoint.AddressFamily ? 0x4 : 0x1),
                               ctx.Client.EndPoint.Address.GetAddressBytes(),
                               (ushort)ctx.Client.EndPoint.Port,
                               toLocal.Memory.Slice(4),
                               out int written))
            {
                toLocal.SignificantLength = 4 + written;
                int payloadToCopy = Math.Min(toLocal.Memory.Length - toLocal.SignificantLength, ctx.Memory.Length);
                ctx.Memory.Slice(0, payloadToCopy).CopyTo(toLocal.Memory.Slice(toLocal.SignificantLength));
                toLocal.SignificantLength += payloadToCopy;

                toLocal.Memory.Span[0] = 0x5;
                toLocal.Memory.Span.Slice(1, 3).Fill(0x0);

                return new PipeFilterResult(ctx.Client, toLocal, true); ;
            }

            return new PipeFilterResult(ctx.Client, toLocal, false); ;
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
    }
}
