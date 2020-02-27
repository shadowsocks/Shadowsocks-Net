/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;
using Argument.Check;


namespace Shadowsocks.Local
{
    public interface IServerLoader
    {
        Server Load(EndPoint target = null);
    }
}
