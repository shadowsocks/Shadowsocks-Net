/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    public interface IReaderFilter<TFilterContext, TFilterResult> : IFilter
        where TFilterContext : IFilterContext
        where TFilterResult : IFilterResult
    {
        TFilterResult OnReading(TFilterContext filterContext);
    }
}
