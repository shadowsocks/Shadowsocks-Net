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
    public class UdpEncapsulationFilter : ClientFilter
    {
        public UdpEncapsulationFilter(ILogger logger = null)
               : base(ClientFilterCategory.Encapsulation, 0, logger)
        {
        }

        public override ClientFilterResult OnReading(ClientFilterContext ctx)
        {
            if (!ctx.Memory.IsEmpty)
            {
                SmartBuffer toApplication = SmartBuffer.Rent(1500);
                ctx.Memory.CopyTo(toApplication.Memory.Slice(2 + 1));
                var p = toApplication.Memory.Span;
                p.Slice(0, 3).Fill(0x0);
                toApplication.SignificantLength = ctx.Memory.Length + 2 + 1;
                return new ClientFilterResult(ctx.Client, toApplication, true);
            }
            else { _logger?.LogError($"LocalUdpRelayPackingFilter OnReading filterContext.Memory.IsEmpty"); }

            return new ClientFilterResult(this.Client, null, false);
        }

        public override ClientFilterResult OnWriting(ClientFilterContext ctx)
        {
            if (!ctx.Memory.IsEmpty)
            {
                SmartBuffer toRemote = SmartBuffer.Rent(1500);
                ctx.Memory.Slice(3).CopyTo(toRemote.Memory);
                toRemote.SignificantLength = ctx.Memory.Length - 3;
                return new ClientFilterResult(ctx.Client, toRemote, true);
            }
            else { _logger?.LogError($"LocalUdpRelayPackingFilter OnWriting filterContext.Memory.IsEmpty"); }

            return new ClientFilterResult(this.Client, null, false);
        }
    }
}
