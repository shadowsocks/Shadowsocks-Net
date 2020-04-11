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
    public class ClientReaderWriterPair : IClientObject
    {

        public IClient Client { protected set; get; }
        public ClientReader Reader { protected set; get; }
        public ClientWriter Writer { protected set; get; }

        public void Set(IClient client, int bufferSize = 8192, ILogger logger = null)
        {
            Client = Throw.IfNull(() => client);

            Reader = new ClientReader(client, false, bufferSize, logger);
            Writer = new ClientWriter(client, true, bufferSize, logger);
        }

        public ClientReaderWriterPair()
        {
        }

    }
}
