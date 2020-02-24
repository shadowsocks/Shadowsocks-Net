/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    /// 1.Don't put too much logic in PipeFilter, although you can do it.
    /// 2.Don't do too much except process data.
    /// 3.Always copy data.
    /// </summary>
    public interface IPipeFilter 
    {
        PipeFilterResult BeforeWriting(PipeFilterContext filterContext);
        PipeFilterResult AfterReading(PipeFilterContext filterContext);
    }
}
