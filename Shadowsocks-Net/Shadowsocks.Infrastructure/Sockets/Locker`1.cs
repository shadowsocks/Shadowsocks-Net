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
using Argument.Check;


namespace Shadowsocks.Infrastructure.Sockets
{
    public sealed class Locker<TClient>
      where TClient : IClient
    {
        public TClient Owner { set; get; }

        public IPEndPoint Number { get; private set; }

        ConcurrentQueue<FixedSizeBuffer> _packets = null;
        const int CAPACITY = 10;

        SemaphoreSlim _notify = null;
        CancellationTokenSource _tokenDestroy = null;

        public Locker(IPEndPoint number)
        {
            Number = Throw.IfNull(() => number);
            _packets = new ConcurrentQueue<FixedSizeBuffer>();
            _notify = new SemaphoreSlim(0);

            _tokenDestroy = new CancellationTokenSource();
        }

        ~Locker()
        {
            Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>packet. null if destroyed or cancelled.</returns>
        public async Task<FixedSizeBuffer> RetrievePacket(CancellationToken cancellationToken)
        {
            if (_tokenDestroy.IsCancellationRequested)//destroyed
            {
                return null;
            }
            await _notify.WaitAsync(Timeout.Infinite, cancellationToken);//wait if no packet

            if (_tokenDestroy.IsCancellationRequested || _packets.IsEmpty)//destroyed
            {
                return null;
            }

            FixedSizeBuffer packet = null;
            while (_packets.Count > 0 && !_packets.TryDequeue(out packet))
            {
            }
            return packet;
        }


        public void PutPacket(FixedSizeBuffer packet)
        {
            if (_tokenDestroy.IsCancellationRequested)//destroyed
            {
                return;
            }
            if (_packets.Count >= CAPACITY)//too many packets to be retrieved.
            {
                //drop the packet.
                packet.Pool.Return(packet);
            }
            else
            {
                _packets.Enqueue(packet);
                _notify.Release(1);
            }
        }

        public void Destroy()
        {
            while (null != _packets && _packets.Count > 0)
            {
                if (_packets.TryDequeue(out FixedSizeBuffer buff))
                {
                    buff.Pool.Return(buff);
                }

            }

            if (null != _tokenDestroy)
            {
                _tokenDestroy.Cancel();
            }
            if (null != _notify)
            {
                _notify.Release(5);
            }
        }
    }
}
