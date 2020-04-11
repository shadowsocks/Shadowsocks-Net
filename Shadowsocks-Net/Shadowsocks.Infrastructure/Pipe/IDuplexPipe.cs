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
    using Sockets;

    /*
      +---------+   +----------+                       +----------+   +---------+ 
      |         |>>>| ReaderA  | >>>>>>>>>>>>>>>>>>>>> | WriterB  |>>>|         |
      | ClientA |   +----------+                       +----------+   | ClientB |
      |         |   +----------+                       +----------+   |         |
      |         |<<<| WriterA  | <<<<<<<<<<<<<<<<<<<<< | ReaderB  |<<<|         |
      +---------+   +----------+                       +----------+   +---------+                                                       
     */

    /// <summary>
    /// A duplex pipe that links two clients and exchanges their data.
    /// </summary>
    public interface IDuplexPipe : IPipe
    {
        IClient ClientA { get; set; }
        IClient ClientB { get; set; }

        IReader GetReader(IClient client);
        IWriter GetWriter(IClient client);

    }
}
