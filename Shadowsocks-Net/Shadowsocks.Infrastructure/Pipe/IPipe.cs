/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;


    /// <summary>
    /// The pipe.
    /// </summary>
    public interface IPipe
    {
        //IClient ClientA { get; }
        //IClient ClientB { get; }

        event EventHandler<PipeEventArgs> OnBroken;
        void Pipe();
        void UnPipe();
    }
}
