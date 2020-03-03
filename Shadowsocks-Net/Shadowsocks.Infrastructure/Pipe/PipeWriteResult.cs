/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Pipe
{
    public struct PipeWriteResult
    {
        public PipeReadWriteResult Result;
        public int Written;

        public PipeWriteResult(PipeReadWriteResult result, int written)
        {
            Result = result;
            Written = written;
        }
    }
}
