/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    public readonly struct PipeFilterContext
    {
        public readonly IClient Client;
        public readonly ReadOnlyMemory<byte> Memory;

        public PipeFilterContext(IClient client, ReadOnlyMemory<byte> memory = default)
        {
            Client = client;
            Memory = memory;           
        }
    }
}
