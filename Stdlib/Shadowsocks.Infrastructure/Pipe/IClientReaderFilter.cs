﻿/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    public interface IClientReaderFilter : IReaderFilter<ClientFilterContext, ClientFilterResult>, IClientObject
    {
    }
}
