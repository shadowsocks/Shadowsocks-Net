/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks_Minimal_Crossplatform_Remote
{
    using Shadowsocks.Infrastructure;
    using Shadowsocks.Infrastructure.Sockets;
    using System.Text;

    class RemoteServer
    {
        //
        static async Task Main(string[] args)
        {
            Init();

            await Task.CompletedTask;
            Console.WriteLine("press key to exit");
            Console.ReadKey();

        }




        public static ILogger logger = null;
        static void Init()
        {
            var config = new ConfigurationBuilder()
                //.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("app-config.json", optional: true, reloadOnChange: true)
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                //.AddNLog(config);
                .AddConsole();
                // .AddDebug();
                //.AddEventLog();
            });
            //https://github.com/NLog/NLog/wiki/Getting-started-with-.NET-Core-2---Console-application#33-update-your-main
            //logger = loggerFactory.CreateLogger<Program>();
            logger = loggerFactory.CreateLogger("SSRemote");
        }
    }
}
