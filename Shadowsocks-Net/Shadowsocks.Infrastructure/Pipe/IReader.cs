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
    /// Represent a data reader.
    /// </summary>
    public interface IReader
    {
        ValueTask<ReadResult> Read(CancellationToken cancellationToken);
    }
}
