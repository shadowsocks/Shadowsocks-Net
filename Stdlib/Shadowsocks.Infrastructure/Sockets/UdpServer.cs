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
    public partial class UdpServer : Server<UdpClient2>
    {
        /// <summary>
        /// 
        /// </summary>
        public const int CLIENT_ACTIVE_TIMEOUT = 30 * 1000;

        ServerConfig _config = null;
        UdpClient _listenerClient = null;

        LruCache<Locker<UdpClient2>> _clientLockers = null;
        FixedSizeBuffer.BufferPool _bufferPool = null;

        public UdpServer(ServerConfig serverConfig, ILogger logger = null)
            : base(logger)
        {
            this._config = Throw.IfNull(() => serverConfig);

            this._clientLockers = new LruCache<Locker<UdpClient2>>();
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
            

            try
            {
                InitializeListener();
                _logger?.LogInformation("UdpServer starting...");
                //_listener.Start(10);
                IsRunning = true;
                _logger?.LogInformation($"UdpServer is listening on [{EndPoint.ToString()}]...");
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
                    _logger?.LogError($"UdpServer socket close error {ex.SocketErrorCode}, {ex.Message}.");
                }                
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"UdpServer socket close error.");
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
                var received = await _listenerClient.Client.ReceiveFromAsync(seg, SocketFlags.None, clientEndPoint);// _listenerClient.Client.LocalEndPoint);
                buff.SignificantLength = received.ReceivedBytes;
                #endregion

                //////var received = await _listenerClient.ReceiveAsync();
                var locker = _clientLockers.Get(received.RemoteEndPoint);
                if (null != locker)//
                {
                    #region delivery packet
                    if ((DateTime.Now - locker.Owner.LastActive).TotalMilliseconds <= CLIENT_ACTIVE_TIMEOUT)
                    {
                        locker.PutPacket(buff);
                        return null;
                    }
                    else//inactive  
                    {
                        DeleteLocker(received.RemoteEndPoint);
                        buff.Pool.Return(buff);//drop packet.
                        return null;
                    }
                    #endregion
                }
                else
                {
                    #region create client
                    locker = new Locker<UdpClient2>(received.RemoteEndPoint as IPEndPoint);
                    locker.PutPacket(buff);
                    KeepLocker(locker);

                    _logger?.LogInformation($"UdpServer New client:[{locker.Number.ToString()}].");

                    return CreateClient(_listenerClient.Client, locker);
                    #endregion
                }

            }
            catch (SocketException se)
            {
                _logger?.LogError($"UdpServer ReceiveFromAsync error {se.SocketErrorCode}, {se.Message}.");
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"UdpServer ReceiveFromAsync error {ex.Message}.");
                return null;
            }

        }


        UdpClient2 CreateClient(Socket sock, Locker<UdpClient2> locker)
        {
            UdpClient2 udpClient2 = new UdpClient2(sock, locker, _logger);
            locker.Owner = udpClient2;
            return udpClient2;
        }

        void KeepLocker(Locker<UdpClient2> locker)
        {
            PostEvictionCallbackRegistration cb = new PostEvictionCallbackRegistration();
            cb.EvictionCallback = (key, value, reason, state) =>
            {
                if (reason == EvictionReason.Expired)//
                {
                    _logger?.LogInformation($"UDP server locker expired:{key.ToString()},{reason.ToString()}");
                    Locker<UdpClient2> locker = value as Locker<UdpClient2>;
                    if ((DateTime.Now - locker.Owner.LastActive).TotalMilliseconds <= CLIENT_ACTIVE_TIMEOUT)
                    {
                        KeepLocker(locker);
                        _logger?.LogInformation($"UDP locker cache readd:{key.ToString()}");
                    }
                    else
                    {
                        locker.Destroy();
                    }
                }

            };
            _clientLockers.Set(locker.Number, locker, TimeSpan.FromMilliseconds(CLIENT_ACTIVE_TIMEOUT), cb);
        }

        void DeleteLocker(EndPoint endPoint)
        {
            if (null != _clientLockers)
            {
                _clientLockers.Remove(endPoint);
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
