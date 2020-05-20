﻿/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Http
{
    using Infrastructure;
    using Infrastructure.Sockets;

    public interface IHttpHandler : IDisposable
    {
        Task HandleHttp(IClient httpClient, CancellationToken cancellationToken = default);
    }
}
