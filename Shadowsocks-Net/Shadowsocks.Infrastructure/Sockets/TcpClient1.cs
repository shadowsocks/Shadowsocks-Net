/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Argument.Check;
using Microsoft.Extensions.Logging;

namespace Shadowsocks.Infrastructure.Sockets
{
    public class TcpClient1 : Client
    {
        public TcpClient1(Socket socket, ILogger logger = null)
           : base(socket, logger)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="logger"></param>
        /// <returns>null if error.</returns>
        /// <exception cref="">no exception</exception>
        public static async Task<TcpClient1> ConnectAsync(IPEndPoint remoteEndPoint, ILogger logger = null)
        {
            Throw.IfNull(() => remoteEndPoint);
            try
            {
                Socket sock = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);                         
                await sock.ConnectAsync(remoteEndPoint);
                sock.ReceiveTimeout = Defaults.ReceiveTimeout;
                sock.SendTimeout = Defaults.SendTimeout;
                sock.ReceiveBufferSize = Defaults.ReceiveBufferSize;
                sock.SendBufferSize = Defaults.SendBufferSize;
                sock.NoDelay = false;

                return new TcpClient1(sock, logger);
            }
            catch (SocketException se)
            {
                //switch (se.SocketErrorCode)
                //{
                //    default:
                //}
                logger?.LogError(se, "TcpClient1 ConnectAsync error.");
                return null;
            }
        }


    }
}
