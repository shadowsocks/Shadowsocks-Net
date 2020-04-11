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


namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    public class ClientReaderPair : IClientReaderAccessor
    {
        public IReader this[IClient client]
        {
            get
            {
                Throw.IfNull(() => client);
                if (null != _readerA && object.ReferenceEquals(_readerA.Client, client)) { return _readerA; }
                else if (null != _readerB && object.ReferenceEquals(_readerB.Client, client)) { return _readerB; }
                else { throw new ArgumentOutOfRangeException("invalid client"); }
            }
        }

        private IReader _readerA = null;
        private IReader _readerB = null;

        public IReader ReaderA
        {
            get => _readerA;
            set
            {
                Throw.IfNull(() => value);
                Throw.IfEqualsTo(() => value, _readerB);
                _readerA = value;
            }
        }
        public IReader ReaderB
        {
            get => _readerB;
            set
            {
                Throw.IfNull(() => value);
                Throw.IfEqualsTo(() => value, _readerA);
                _readerB = value;
            }
        }

        public ClientReaderPair(IReader readerA, IReader readerB)
        {
            Throw.IfNull(() => readerA);
            Throw.IfNull(() => readerB);

            Throw.IfEqualsTo(() => readerA, readerB);
            Throw.IfEqualsTo(() => readerA.Client, readerB.Client);

            _readerA = readerA;
            _readerB = readerB;
        }

        public ClientReaderPair() { }


    }
}
