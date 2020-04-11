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
using System.Collections;

namespace Shadowsocks.Infrastructure.Pipe
{
    /// <summary>
    /// A sorted filter list.
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    public sealed class FilterChain<TFilter> : IEnumerable<TFilter>
        where TFilter : class, IFilter
    {
        SortedSet<TFilter> _chain = null;
        IEnumerable<TFilter> _chainReversed = null;
        bool _reversed = false;


        public int Count => _chain.Count;

        public FilterChain(bool reversed = false)
        {
            _chain = new SortedSet<TFilter>();
            _reversed = reversed;
        }


        public void Add(TFilter filter)
        {
            Throw.IfNull(() => filter);

            if (_chain.Contains(filter))
            {
                _chain.Add(filter);
            }

            CopyReverse();
        }


        public void Remove(TFilter filter)
        {
            if (_chain.Contains(filter))
            {
                _chain.Remove(filter);
            }

            CopyReverse();
        }

        public bool Contains(TFilter filter)
        {
            return _chain.Contains(filter);
        }



        public IEnumerator<TFilter> GetEnumerator()
        {
            return _reversed ? _chainReversed.GetEnumerator() : _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _reversed ? _chainReversed.GetEnumerator() : _chain.GetEnumerator();
        }

        void CopyReverse()
        {
            _chainReversed = _chain.Reverse();
        }
    }
}
