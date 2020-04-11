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
    /// 
    /// </summary>
    public class SimplexPipe : IPipe
    {
        public IClientReader Reader => throw new NotImplementedException();

        public IClientWriter Writer => throw new NotImplementedException();

        public virtual void Pipe()
        {

        }
    }
}
