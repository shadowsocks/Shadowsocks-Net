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
using System.IO;
using Argument.Check;

namespace Shadowsocks_Minimal_Crossplatform_Remote
{
    using Shadowsocks;
    using Shadowsocks.Remote;
    using Shadowsocks.Infrastructure;
    using Shadowsocks.Infrastructure.Sockets;

    class Program
    {

        static RemoteServer remoteServer = null;

        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("app-config.json", optional: true, reloadOnChange: true).Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole();  //.AddNLog(config);
            });



            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            if (remoteServer == null)
            {
                var remoteConfig = JsonSerializer.Deserialize<RemoteServerConfig>(
                    File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")), options);
                //var logger = loggerFactory.CreateLogger<RemoteServer>();
                var logger = loggerFactory.CreateLogger("Remote");

                remoteServer = new RemoteServer(remoteConfig, logger);
            }
            remoteServer.Start();
            await Task.CompletedTask;

            Console.WriteLine("press key to stop server");
            Console.ReadKey();
            remoteServer.Stop();

            Console.WriteLine("press key to exit");
            Console.ReadKey();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            //remoteServer.Stop();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {

        }


    }

}
