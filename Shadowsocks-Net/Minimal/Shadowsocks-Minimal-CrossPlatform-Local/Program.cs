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

namespace Shadowsocks_Minimal_Crossplatform_Local
{
    using Shadowsocks;
    using Shadowsocks.Local;
    using Shadowsocks.Infrastructure;
    using Shadowsocks.Infrastructure.Sockets;

    class Program
    {
        static LocalServer localServer = null;
        static Shadowsocks.Http.HttpProxyServer httpProxyServer = null;

        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("app-config.json", optional: true, reloadOnChange: true).Build();
            var socks5Config = config.GetSection("Socks5Proxy").Get<LocalServerConfig>();
            var httpConfig = config.GetSection("HttpProxy").Get<Shadowsocks.Http.HttpProxyServerConfig>();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder//.AddConfiguration(config)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole();  //.AddNLog(config);
            });

            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;


            //var logger = loggerFactory.CreateLogger<LocalServer>();
            var logger = loggerFactory.CreateLogger("Local");
            var serverLoader = new DefaultServerLoader();
            if (null == localServer)
            {
                localServer = new LocalServer(socks5Config, serverLoader, logger);
            }                      
            if (null == httpProxyServer)
            {
                httpProxyServer = new Shadowsocks.Http.HttpProxyServer(httpConfig, serverLoader, logger);
            }
            localServer.Start();
            httpProxyServer.Start();
            
           
            await Task.CompletedTask;

            Console.WriteLine("press any key to stop server");           
            Console.ReadKey();
            localServer.Stop();
            httpProxyServer.Stop();

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
           
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {

        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {

        }

    }


}
