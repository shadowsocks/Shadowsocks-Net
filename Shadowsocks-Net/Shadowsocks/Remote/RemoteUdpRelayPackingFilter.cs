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
        MemoryWriter _memoryWriter = null;
        public RemoteUdpRelayPackingFilter(IClient udpClient, ILogger logger = null)
               : base(udpClient, 20, logger)
        {

        }

        //Applies to clients at both ends of the tunnel.

        public override PipeFilterResult AfterReading(PipeFilterContext ctx)
        {
            SmartBuffer toLocal = SmartBuffer.Rent(1500);//TODO what if exceeds 1500? fragments or not?
            _memoryWriter ??= new MemoryWriter(toLocal.Memory);
            _memoryWriter.ResetMemory(toLocal.Memory);

            var addrBytes = ctx.Client.EndPoint.Address.GetAddressBytes();
            var portBytes= BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short) ctx.Client.EndPoint.Port));
            Span<byte> b2 = stackalloc byte[2];
            b2[0] = (byte)(AddressFamily.InterNetworkV6 == ctx.Client.EndPoint.Address.AddressFamily ? 0x4 : 0x1);
            b2[1] = (byte)addrBytes.Length;
            _memoryWriter.Write(b2);
            _memoryWriter.Write(addrBytes.AsSpan());
            _memoryWriter.Write(portBytes.AsSpan());
            _memoryWriter.Write(ctx.Memory.Slice(0, Math.Min(ctx.Memory.Length, _memoryWriter.Length - _memoryWriter.Position)));
            toLocal.SignificantLength = _memoryWriter.Position;

            return new PipeFilterResult(ctx.Client, toLocal, true); ;
        }

        public override PipeFilterResult BeforeWriting(PipeFilterContext ctx)
        {
            SmartBuffer toTarget = SmartBuffer.Rent(ctx.Memory.Length);
            int payloadOffset = 1 + 1 + ctx.Memory.Span[1] + 2;
            ctx.Memory.Slice(payloadOffset).CopyTo(toTarget.Memory);
            toTarget.SignificantLength = ctx.Memory.Length - payloadOffset;
            return new PipeFilterResult(ctx.Client, toTarget, true);
        }
    }
}
