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
        public ObfuscationFilter(IClient client, ILogger logger = null)
               : base(client, ClientFilterCategory.Obfuscation, 0)
        {
            _logger = logger;
        }

        public override ClientFilterResult AfterReading(ClientFilterContext filterContext)
        {
            throw new NotImplementedException();
        }

        public override ClientFilterResult BeforeWriting(ClientFilterContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
