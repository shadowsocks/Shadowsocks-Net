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
using System.Buffers;
using Argument.Check;
using Microsoft.Extensions.Logging;

namespace Shadowsocks.Obfuscation
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;


    public class ObfuscationFilter : ClientFilter
    {
        public ObfuscationFilter(ILogger logger = null)
               : base(ClientFilterCategory.Obfuscation, 0, logger)
        {            
        }

        public override ClientFilterResult OnReading(ClientFilterContext filterContext)
        {
            throw new NotImplementedException();
        }

        public override ClientFilterResult OnWriting(ClientFilterContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
