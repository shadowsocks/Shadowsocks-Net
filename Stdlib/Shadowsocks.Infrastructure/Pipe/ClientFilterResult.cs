/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    public readonly struct ClientFilterResult : IFilterResult, IClientObject
    {
        public readonly IClient Client { get; }
        public readonly SmartBuffer Buffer { get; }
        public readonly bool Continue { get; }

        public ClientFilterResult(IClient client, SmartBuffer buffer = null, bool @continue = true)
        {
            Client = client;
            Buffer = buffer;
            Continue = @continue;
        }
    }
}
