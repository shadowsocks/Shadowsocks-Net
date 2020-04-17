/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Sockets
{
    public sealed class TcpServer : Server<TcpClient1>
    {
        ServerConfig _config = null;
        TcpListener _listener = null;

        public TcpServer(ServerConfig serverConfig, ILogger logger = null)
            : base(logger)
        {
            this._config = Throw.IfNull(() => serverConfig);
        }

        ~TcpServer()
        {
            StopListen();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Listen()
        {
            if (IsRunning) { return; }
            StopListen();
           
            try
            {
                InitializeListener();
                _logger?.LogInformation("TcpServer starting...");
                _listener.Start(10);
                IsRunning = true;
                _logger?.LogInformation($"TcpServer is listening on [{EndPoint.ToString()}]...");

                ////Task.Run(async () => { await StartAccept(); });
            }
            catch (SocketException se)
            {
                IsRunning = false;
                _logger?.LogError($"TcpServer socket listen failed {se.SocketErrorCode}, {se.Message}.");
            }
            catch(Exception ex)
            {
                IsRunning = false;
                _logger?.LogError(ex, $"TcpServer socket listen failed.");
            }
        }

        public override void StopListen()
        {
            if (!IsRunning) { return; }
            if (null != _listener)
            {
                try
                {
                    _logger?.LogInformation("TcpServer socket closing...");
                    /*
                     * Stop() closes the listener. Any unaccepted connection requests in the queue will be lost. 
                     * Remote hosts waiting for a connection to be accepted will throw a SocketException. 
                     * You are responsible for closing your accepted connections separately.
                     * The Stop() method also closes the underlying Socket, and creates a new Socket for the TcpListener. 
                     * If you set any properties on the underlying Socket prior to calling the Stop() method, 
                     * those properties will not carry over to the new Socket.
                     */

                    _listener.Stop();
                    _listener = null;
                    _logger?.LogInformation("TcpServer socket closed.");
                }
                catch (SocketException se)
                {
                    _logger?.LogError($"TcpServer socket close error {se.SocketErrorCode}, {se.Message}.");
                }
                finally
                {
                    _listener = null;
                }
            }
            IsRunning = false;
            _logger?.LogInformation("TcpServer stopped.");

        }


        /// <summary>
        /// Try to accept a client.
        /// </summary>
        /// <returns>A client if succeed, otherwise, null.</returns>
        /// <exception cref="">no exception</exception>
        public override async Task<TcpClient1> Accept()//TODO MaxNumClient limit. //TODO blocklist.
        {
            if (!IsRunning) { return null; }
            try
            {
                var sock = await _listener.AcceptSocketAsync();
                var client = Accept(sock);
                return client;

            }
            catch (SocketException se)
            {
                _logger?.LogError(se, $"TcpServer Accept error {se.ErrorCode}");
                return null;
            }
        }



        TcpClient1 Accept(Socket sock)
        {
            sock.LingerState = new LingerOption(true, 5);
            sock.NoDelay = false;
            //sock.SendTimeout = _config.SendTimeout;
            //sock.ReceiveTimeout = _config.ReceiveTimeout;

            TcpClient1 tcpClient1 = new TcpClient1(sock, this._logger);
            return tcpClient1;
        }



        void InitializeListener()
        {
            if (null == _listener)
            {
                this.EndPoint = this._config.BindPoint;
               
                _listener = new TcpListener(this.EndPoint);
                _listener.ExclusiveAddressUse = false;
            }

        }


    }
}
