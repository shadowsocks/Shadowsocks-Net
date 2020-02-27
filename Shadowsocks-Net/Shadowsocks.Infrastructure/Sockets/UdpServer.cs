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
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;
using Microsoft.Extensions.Caching.Memory;

namespace Shadowsocks.Infrastructure.Sockets
{
    public partial class UdpServer : Server<UdpServer.UdpClient2>
    {
        /// <summary>
        /// 
        /// </summary>
        public const int CLIENT_ACTIVE_TIMEOUT = 10 * 1000;

        ILogger _logger = null;
        ServerConfig _config = null;
        UdpClient _listenerClient = null;

        LruCache<UdpClient2> _clientManager = null;
        FixedSizeBuffer.BufferPool _bufferPool = null;
        public UdpServer(ServerConfig serverConfig, ILogger logger = null)
        {
            this._config = Throw.IfNull(() => serverConfig);
            this._logger = logger;

            this._clientManager = new LruCache<UdpClient2>();
            this._bufferPool = new FixedSizeBuffer.BufferPool(1500, 10 * Defaults.MaxNumClient, 5);

        }
        ~UdpServer()
        {
            StopListen();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Listen()
        {
            if (IsRunning) { return; }
            StopListen();
            InitializeListener();

            try
            {
                _logger?.LogInformation("UdpServer starting...");
                //_listener.Start(10);
                IsRunning = true;
                _logger?.LogInformation("UdpServer is listening...");
            }
            catch (Exception ex)
            {
                IsRunning = false;
                _logger?.LogError(ex, "UdpServer socket listen failed.");
            }
        }

        public override void StopListen()
        {
            if (!IsRunning) { return; }
            if (null != _listenerClient)
            {

                try
                {
                    _logger?.LogInformation("UdpServer socket closing...");
                    /*
                     * The Close disables the underlying Socket and releases all managed 
                     * and unmanaged resources associated with the UdpClient.
                     */

                    _listenerClient.Close();
                    _listenerClient = null;
                    _logger?.LogInformation("UdpServer socket closed.");
                }
                catch (SocketException ex)
                {
                    _logger?.LogError(ex, "UdpServer socket close error.");
                }
                finally
                {
                    _listenerClient = null;
                }
            }
            IsRunning = false;
            _logger?.LogInformation("UdpServer stopped.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>usually null.</returns>
        public override async Task<UdpClient2> Accept()//TODO MaxNumClient limit. //TODO blocklist.
        {
            if (null == _listenerClient) { return null; }
            try
            {
                #region receive packet
                FixedSizeBuffer buff = _bufferPool.Rent();
                ArraySegment<byte> seg = new ArraySegment<byte>(buff.Memory, buff.Offset, buff.Memory.Length);

                //TODO cache
                IPEndPoint clientEndPoint = new IPEndPoint(_listenerClient.Client.LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6 ?
                    IPAddress.IPv6Any : IPAddress.Any, 0);
                var result = await _listenerClient.Client.ReceiveFromAsync(seg, SocketFlags.None, clientEndPoint);// _listenerClient.Client.LocalEndPoint);
                buff.SignificantLength = result.ReceivedBytes;
                #endregion

                //////var result = await _listenerClient.ReceiveAsync();
                var client = _clientManager.Get(result.RemoteEndPoint);
                if (null != client)//
                {
                    #region delivery packet
                    if (client.Active)
                    {
                        client.PostReceived(buff);
                        return null;
                    }
                    else//inactive  
                    {
                        DeleteClient(result.RemoteEndPoint);
                        buff.Pool.Return(buff);//drop packet.
                        return null;
                    }
                    #endregion
                }
                else
                {
                    #region create client
                    client = CreateClient(_listenerClient.Client, result.RemoteEndPoint as IPEndPoint);
                    client.PostReceived(buff);
                    KeepClient(client);

                    _logger?.LogInformation($"UdpServer New client:[{client.EndPoint.ToString()}].");
                    return client;
                    #endregion
                }

            }
            catch (SocketException se)
            {
                _logger?.LogError(se, "UdpServer ReceiveFromAsync error.");
                return null;
            }


        }


        UdpClient2 CreateClient(Socket sock, IPEndPoint remoteIP)
        {
            UdpClient2 udpClient2 = new UdpClient2(sock, remoteIP, _logger);
            return udpClient2;
        }

        void KeepClient(UdpClient2 client)
        {
            PostEvictionCallbackRegistration cb = new PostEvictionCallbackRegistration();
            cb.EvictionCallback = (key, value, reason, state) =>
            {
                if (reason == EvictionReason.Expired)//
                {
                    _logger?.LogInformation($"UDP cache client expired:{key.ToString()},{reason.ToString()}");
                    UdpClient2 c = value as UdpClient2;
                    if (c.Active)
                    {
                        KeepClient(c);
                        _logger?.LogInformation($"UDP cache client readd:{key.ToString()}");
                    }
                }

            };
            _clientManager.Set(client.EndPoint, client, TimeSpan.FromMilliseconds(CLIENT_ACTIVE_TIMEOUT), cb);
        }

        void DeleteClient(EndPoint endPoint)
        {
            if (null != _clientManager)
            {
                _clientManager.Remove(endPoint);
            }
        }

        void InitializeListener()
        {
            if (null == _listenerClient)
            {
                this.EndPoint = _config.BindPoint;

                _listenerClient = new UdpClient();
                _listenerClient.ExclusiveAddressUse = false;
                _listenerClient.DontFragment = true;
                _listenerClient.EnableBroadcast = false;

                _listenerClient.Client.Bind(this.EndPoint);
            }

        }
    }
}
