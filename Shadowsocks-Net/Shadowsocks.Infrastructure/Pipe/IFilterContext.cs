/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    /// Filter context for Reading / Writing.
    /// </summary>
    public interface IFilterContext
    {
        ReadOnlyMemory<byte> Memory { get; }
    }
}
