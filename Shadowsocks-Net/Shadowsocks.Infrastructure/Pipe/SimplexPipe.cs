/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections;
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
    using Sockets;
    using static ReadWriteResult;

    /// <summary>
    /// 
    /// </summary>
    public class SimplexPipe : ISimplexPipe<ClientReader, ClientWriter>
    {
        public virtual ClientReader Reader { protected set; get; }

        public virtual ClientWriter Writer { protected set; get; }




        protected ILogger _logger = null;
        public SimplexPipe(ClientReader clientReader, ClientWriter clientWriter, ILogger logger = null)
            : this(logger)
        {
            Reader = Throw.IfNull(() => clientReader);
            Writer = Throw.IfNull(() => clientWriter);
        }

        protected SimplexPipe(ILogger logger = null)
        {
            _logger = logger;
        }

        public virtual async ValueTask<PipeResult> Pipe(CancellationToken cancellationToken)
        {
            PipeResult rt = new PipeResult { Broken = false, BrokenCause = PipeBrokenCause.Empty };
            var readerResult = await Reader.Read(cancellationToken);
            if (Failed == readerResult.Result)
            {
                return new PipeResult { Broken = true, BrokenCause = PipeBrokenCause.Exception };
            }
            if (BrokeByFilter == readerResult.Result)
            {
                _logger?.LogInformation($"Pipe broke by Reader filter:[{Reader.Client.EndPoint}].");
                return new PipeResult { Broken = true, BrokenCause = PipeBrokenCause.FilterBreak };
            }

            if (readerResult.Read > 0)//happens sometimes, [AfterReading] filter may not return data.
            {
                var writeResult = await Writer.Write(readerResult.Memory.SignificantMemory, cancellationToken);
                if (Failed == writeResult.Result)
                {
                    return new PipeResult { Broken = true, BrokenCause = PipeBrokenCause.Exception };
                }
                if (BrokeByFilter == writeResult.Result)
                {
                    _logger?.LogInformation($"Pipe broke by Writer filter: [{Writer.Client.EndPoint}].");
                    return new PipeResult { Broken = true, BrokenCause = PipeBrokenCause.FilterBreak };
                }
                _logger?.LogInformation($"Pipe [{Reader.Client.EndPoint}] => [{Writer.Client.EndPoint}] {writeResult.Written} bytes.");
                //ReportPiping(new PipingEventArgs { Bytes = writeResult.Written, Origin = ClientA.EndPoint, Destination = ClientB.EndPoint });

                rt = new PipeResult { Broken = false, BrokenCause = PipeBrokenCause.Empty, BytesPiped = writeResult.Written };

            }
            readerResult.Memory?.Dispose();

            return rt;
        }



    }
}
