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
    /// Represent a data writer.
    /// </summary>
    public interface IWriter
    {
        ValueTask<WriteResult> Write(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
    }
}
