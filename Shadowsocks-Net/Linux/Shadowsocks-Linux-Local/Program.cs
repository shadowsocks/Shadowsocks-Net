/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Buffers;
using System.Linq;
using System.IO;
using Argument.Check;

namespace Shadowsocks_Linux_Remote
{
    using Shadowsocks;
    using Shadowsocks.Local;
    using Shadowsocks.Infrastructure;
    using Shadowsocks.Infrastructure.Sockets;

    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
