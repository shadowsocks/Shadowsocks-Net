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

namespace Shadowsocks.Local
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;


    /*
    # SOCKS5 UDP Request
    # +----+------+------+----------+----------+----------+
    # |RSV | FRAG | ATYP | DST.ADDR | DST.PORT |   DATA   |
    # +----+------+------+----------+----------+----------+
    # | 2  |  1   |  1   | Variable |    2     | Variable |
    # +----+------+------+----------+----------+----------+

    # SOCKS5 UDP Response
    # +----+------+------+----------+----------+----------+
    # |RSV | FRAG | ATYP | DST.ADDR | DST.PORT |   DATA   |
    # +----+------+------+----------+----------+----------+
    # | 2  |  1   |  1   | Variable |    2     | Variable |
    # +----+------+------+----------+----------+----------+

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
    /// Applies to UDP relay client on local side.
    /// </summary>
    public class LocalUdpRelayPackingFilter : PipeFilter
    {

        public LocalUdpRelayPackingFilter(IClient udpClient, ILogger logger = null)
               : base(udpClient, 20, logger)
        {

        }
        
        public override PipeFilterResult AfterReading(PipeFilterContext ctx)
        {
            SmartBuffer toApplication = SmartBuffer.Rent(1500);
            ctx.Memory.CopyTo(toApplication.Memory.Slice(2 + 1));
            var p = toApplication.Memory.Span;
            p.Slice(0, 3).Fill(0x0);
            toApplication.SignificantLength = ctx.Memory.Length + 2 + 1;
            return new PipeFilterResult(ctx.Client, toApplication, true);

        }

        public override PipeFilterResult BeforeWriting(PipeFilterContext ctx)
        {
            SmartBuffer toRemote = SmartBuffer.Rent(1500);
            ctx.Memory.Slice(3).CopyTo(toRemote.Memory);
            toRemote.SignificantLength = ctx.Memory.Length - 3;
            return new PipeFilterResult(ctx.Client, toRemote, true);
        }
    }
}
