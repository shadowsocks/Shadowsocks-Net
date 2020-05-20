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
    using static ReadWriteResult;

    /// <summary>
    /// A client Reader / Writer with filter support.
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    public abstract class ClientReaderWriter<TFilter> : IClientObject
         where TFilter : class, IFilter, IClientObject
    {
        /// <summary>
        /// The client.
        /// </summary>
        public IClient Client { get; protected set; }

        /// <summary>
        /// Filter chain.
        /// </summary>
        public FilterChain<TFilter> FilterChain { get; protected set; }

        public virtual bool FilterEnabled { set; get; }


        protected int _bufferSize = 8192;
        protected ILogger _logger = null;

        public ClientReaderWriter(IClient client, bool reverseFilterChain = false, int? bufferSize = 8192, ILogger logger = null)
        {
            Client = Throw.IfNull(() => client);
            FilterChain = new FilterChain<TFilter>(reverseFilterChain);
            FilterEnabled = true;

            _bufferSize = bufferSize ?? 8192;
            _logger = logger;
        }



        /// <summary>
        /// Add a filter to filter chain.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(TFilter filter)//TODO lock
        {
            Throw.IfNull(() => filter);
            Throw.IfNotEqualsTo(()=>filter.Client, this.Client);

            if (!FilterChain.Contains(filter))
            {
                FilterChain.Add(filter);
            }
        }
       


    }
}
