/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Cipher
{
    public abstract class ShadowsocksCipher
    {
        public string Password { get; protected set; }//emm..
        public ShadowsocksCipher(string password)
        {
            Password = Throw.IfNullOrEmpty(() => password);
        }
    }
}
