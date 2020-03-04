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
    public interface IClientWriter : IClientObject
    {
        ValueTask<ClientWriteResult> Write(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
    }
}
