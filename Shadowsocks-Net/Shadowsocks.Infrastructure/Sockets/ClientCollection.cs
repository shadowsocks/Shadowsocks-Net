/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// A dictionary organizes clients by remote IPEndPoint. Thread-safe.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public class ClientCollection<TClient> : IEnumerable<TClient>
         where TClient : IClient
    {
        ConcurrentDictionary<IPEndPoint, TClient> _clients = new ConcurrentDictionary<IPEndPoint, TClient>();

        public int Count { get { return _clients.Count; } }


        public TClient this[IPEndPoint remoteIPEndPoint] { get { return (null != remoteIPEndPoint && _clients.ContainsKey(remoteIPEndPoint)) ? _clients[remoteIPEndPoint] : default; } }


        public void Add(TClient client)
        {
            if (null != client)
            {
                while(!_clients.TryAdd(client.EndPoint, client) && !_clients.ContainsKey(client.EndPoint))
                {
                }               
            }
        }

        public void Remove(IPEndPoint remoteIPEndPoint)//
        {
            while (!_clients.TryRemove(remoteIPEndPoint, out TClient c) && _clients.ContainsKey(remoteIPEndPoint))
            {
            }
        }


        public void Clear()
        {
            if (_clients.IsEmpty) { return; }            
            _clients.Clear();
        }

        public IEnumerator<TClient> GetEnumerator()
        {
            return _clients.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _clients.Values.GetEnumerator();
        }
    }
}
