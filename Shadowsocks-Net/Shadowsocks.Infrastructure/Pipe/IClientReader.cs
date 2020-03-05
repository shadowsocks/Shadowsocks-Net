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
    /// Represent a reader that read data from <see cref="Sockets.IClient"/>.
    /// </summary>
    public interface IClientReader : IClientObject
    {
        ValueTask<ClientReadResult> Read(CancellationToken cancellationToken);
    }
}
