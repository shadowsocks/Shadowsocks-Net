/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    ///  Pipe data from <see cref="Reader"/> to <see cref="Writer"/>.
    /// </summary>
    /// <typeparam name="TReader"></typeparam>
    /// <typeparam name="TWriter"></typeparam>
    public interface ISimplexPipe<TReader, TWriter> : IPipe
        where TReader : IReader
        where TWriter : IWriter
    {
        TReader Reader { get; }
        TWriter Writer { get; }
        ValueTask<PipeResult> Pipe(CancellationToken cancellationToken);
    }
}
