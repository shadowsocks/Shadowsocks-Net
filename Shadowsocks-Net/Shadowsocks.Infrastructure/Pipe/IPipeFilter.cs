/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    /// Don't put too much logic in PipeFilter, although you can do it.
    /// Don't do too much except process data.
    /// </summary>
    public interface IPipeFilter 
    {
        PipeFilterResult BeforeWriting(PipeFilterContext filterContext);
        PipeFilterResult AfterReading(PipeFilterContext filterContext);
    }
}
