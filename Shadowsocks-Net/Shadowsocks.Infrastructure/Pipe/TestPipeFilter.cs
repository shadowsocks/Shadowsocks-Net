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
using System.Linq;
using Argument.Check;


namespace Shadowsocks.Infrastructure.Pipe
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;
    public class TestPipeFilter : PipeFilter
    {
        public TestPipeFilter(IClient client, ILogger logger = null)
                 : base(client, 20, logger)
        {
        }
        public override PipeFilterResult BeforeWriting(PipeFilterContext ctx)
        {
            byte[] data = ctx.Memory.ToArray();
            byte[] newData = new byte[data.Length + 4];
            data[0] = 0x12;
            data[1] = 0x34;
            data[2] = 0xab;
            data[3] = 0xcd;
            Array.Copy(data, 0, newData, 4, data.Length);
            return new PipeFilterResult(ctx.Client, newData, ...);
        }

        public override PipeFilterResult AfterReading(PipeFilterContext ctx)
        {
            byte[] data = ctx.Memory.ToArray();
            byte[] newDat = data.Skip(4).ToArray();
            return new PipeFilterResult(ctx.Client, newData, ...);
        }


    }
}
