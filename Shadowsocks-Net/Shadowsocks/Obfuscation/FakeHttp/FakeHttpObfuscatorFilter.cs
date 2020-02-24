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

namespace Shadowsocks.Obfuscation.FakeHttp
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;

    [Obfuscator("fake_http")]
    public class FakeHttpObfuscatorFilter : PipeFilter
    {
        public FakeHttpObfuscatorFilter(IClient client, ILogger logger = null)
               : base(client, 0, logger)
        {

        }

        public override PipeFilterResult AfterReading(PipeFilterContext filterContext)
        {
            throw new NotImplementedException();
        }

        public override PipeFilterResult BeforeWriting(PipeFilterContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
