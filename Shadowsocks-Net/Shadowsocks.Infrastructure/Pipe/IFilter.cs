/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    /// Reading / Writing filter.
    /// </summary>
    public interface IFilter : IComparer<IFilter>
    {
    }
}
