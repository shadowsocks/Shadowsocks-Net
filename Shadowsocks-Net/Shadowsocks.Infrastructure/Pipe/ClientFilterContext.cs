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

    /// <summary>
    /// Stores contextual data traveling through filters. Each filter should copy memory from context when needed, rather than reference it.
    /// </summary>
    public readonly struct ClientFilterContext
    {
        public readonly IClient Client;
        public readonly ReadOnlyMemory<byte> Memory;

        public ClientFilterContext(IClient client, ReadOnlyMemory<byte> memory = default)
        {
            Client = client;
            Memory = memory;           
        }
    }
}
