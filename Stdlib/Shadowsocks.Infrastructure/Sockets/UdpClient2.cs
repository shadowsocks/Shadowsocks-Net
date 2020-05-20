/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Buffers;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// Represent a UDP client that is recognized by UDP server and shares the same socket with the server.
    /// It's most important member is EndPoint, but it also has ReadAsync and WriteAsync, and acts like a real "client".
    /// </summary>
    public sealed class UdpClient2 : Client
    {
        public override IPEndPoint EndPoint => _locker.Number;

        public DateTime LastActive { get; private set; }


        Locker<UdpClient2> _locker = null;
        volatile bool _closed = false;

        public UdpClient2(Socket serverSocket, Locker<UdpClient2> locker, ILogger logger = null)
            : base(serverSocket, logger)
        {
            _sock = Throw.IfNull(() => serverSocket);
            _locker = Throw.IfNull(() => locker);

            _logger = logger;

            UpdateLastActive();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>-1 if error.</returns>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_closed) { return -1; }
            UpdateLastActive();

            var packet = await _locker.RetrievePacket(cancellationToken);
            if (null == packet) { return -1; }

            int toCopy = Math.Min(buffer.Length, packet.SignificantLength);
            packet.Memory.CopyTo(buffer.Slice(0, toCopy));
            packet.Pool.Return(packet);//<-

            return toCopy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>-1 if error.</returns>
        public override async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_closed) { return -1; }
            UpdateLastActive();

            int written = -1;
            try
            {
                using (var arr = RecyclableByteArray.Rent(buffer.Length))
                {
                    buffer.CopyTo(arr.Array);
                    written = await _sock.SendToAsync(new ArraySegment<byte>(arr.Array), SocketFlags.None, _locker.Number);//TODO waiting for a overload.
                }
            }
            catch (SocketException ex)
            {
                _logger?.LogError($"UdpClient2 SendToAsync error {ex.SocketErrorCode}, {ex.Message}.");
                return -1;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"UdpClient2 SendToAsync error {ex.Message}.");
                return -1;
            }
            return written;

        }
        void UpdateLastActive()
        {
            LastActive = DateTime.Now;
        }

        public override void Close()
        {
            ///////////base.Close();
            _closed = true;

            FireClosing();
            _logger?.LogInformation("UdpClient2 Close.");
        }
    }

  


}