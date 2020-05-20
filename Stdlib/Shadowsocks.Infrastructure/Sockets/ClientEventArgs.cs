/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Sockets
{

    public readonly struct ClientEventArgs
    {
        public readonly IClient Client;
        public ClientEventArgs(IClient client)
        {
            this.Client = client;
        }
    }

    public readonly struct ClientReadWriteEventArgs
    {
        public readonly IClient Client;
        public readonly Memory<byte> Buffer;
        public readonly int BytesTransferred;
        public ClientReadWriteEventArgs(IClient client, Memory<byte> buffer, int bytesTransferred)
        {
            this.Client = client;
            this.Buffer = buffer;
            this.BytesTransferred = bytesTransferred;
        }
    }

}
