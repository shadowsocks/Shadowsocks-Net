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
    public partial class UdpServer
    {
        /// <summary>
        /// Represent a UDP client that is recognized by UDP server and shares the same socket with the server.
        /// It's most important member is EndPoint, but it also has ReadAsync and WriteAsync, and acts like a real "client".
        /// </summary>
        public sealed class UdpClient2 : Client
        {            
            public bool Active => _lastActive.AddMilliseconds(UdpServer.CLIENT_ACTIVE_TIMEOUT) >= DateTime.Now;
            DateTime _lastActive = DateTime.Now;

            public override IPEndPoint EndPoint => _remote;
            IPEndPoint _remote = null;

            #region ReceiveBuffer
            ConcurrentQueue<FixedSizeBuffer> _receivedPackets = null;
            const int MAX_BUFFE_COUNT = 10;
            SemaphoreSlim _semaphoreReceived = new SemaphoreSlim(0);
            #endregion

            public UdpClient2(Socket socket, IPEndPoint remoteIPEndPoint, ILogger logger = null)
                : base(socket, logger)
            {
                _remote = Throw.IfNull(() => remoteIPEndPoint);
                _receivedPackets = new ConcurrentQueue<FixedSizeBuffer>();

                _lastActive = DateTime.Now;
            }
            ~UdpClient2()
            {
                Close();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="cancellationToken"></param>
            /// <returns>-1 if error. 0 if read timeout.</returns>
            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                UpdateLastActive();
                if (_closed) { return -1; }

                int read = -1;
                if (null != _semaphoreReceived && await _semaphoreReceived.WaitAsync(Timeout.Infinite))
                {
                    //_logger?.LogInformation("UdpClient2 _semaphoreReceived.WaitAsync()...");
                    if (_receivedPackets.IsEmpty)//timeout.
                    {
                        return Active ? 0 : -1;
                    }

                    FixedSizeBuffer fixedSizeBuffer = null;
                    while (!_receivedPackets.TryDequeue(out fixedSizeBuffer) && _receivedPackets.Count > 0)
                    {
                    }
                    if (null != fixedSizeBuffer)
                    {
                        Memory<byte> mem = new Memory<byte>(fixedSizeBuffer.Memory, fixedSizeBuffer.Offset, fixedSizeBuffer.SignificantLength);
                        if (mem.TryCopyTo(buffer))
                        {
                            read = mem.Length;
                        }
                        fixedSizeBuffer.Pool.Return(fixedSizeBuffer);
                    }

                }
                return read;

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public override async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                UpdateLastActive();
                if (_closed) { return -1; }
                int written = -1;
                try
                {
                    var arr = ArrayPool<byte>.Shared.Rent(buffer.Length);
                    buffer.CopyTo(arr.AsMemory());
                    written = await _sock.SendToAsync(new ArraySegment<byte>(arr), SocketFlags.None, _remote);//TODO waiting for a overload.
                    ArrayPool<byte>.Shared.Return(arr, true);
                }
                catch (SocketException ex)
                {
                    _logger?.LogError(ex, "UdpClient2 SendToAsync error.");
                    return -1;
                }
                return written;
            }

            public void PostReceived(FixedSizeBuffer buffer)
            {
                if (_receivedPackets.Count >= MAX_BUFFE_COUNT)//too many packets to be read.
                {
                    //drop the packet.
                    buffer.Pool.Return(buffer);
                }
                else
                {
                    _receivedPackets.Enqueue(buffer);
                    _semaphoreReceived.Release();
                    //_logger?.LogInformation("UdpClient2 _semaphoreReceived.Release().");
                }
            }

            public void PostExired()
            {
                _semaphoreReceived.Release(2);
            }

            void UpdateLastActive()
            {
                _lastActive = DateTime.Now;
            }

            volatile bool _closed = false;
            public override void Close()
            {
                ///////////base.Close();

                Cleanup();
                _closed = true;

                FireClosing();
                _logger?.LogInformation("UdpClient2 Close.");
            }

            void Cleanup()
            {
                if (null != _semaphoreReceived)
                {
                    _semaphoreReceived.Release(2);
                    _semaphoreReceived.Dispose();
                    _semaphoreReceived = null;
                }

                while (_receivedPackets.Count > 0)
                {
                    if (_receivedPackets.TryDequeue(out FixedSizeBuffer buff))
                    {
                        buff.Pool.Return(buff);
                    }

                }
            }
        }




    }
}