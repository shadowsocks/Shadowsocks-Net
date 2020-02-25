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

namespace Shadowsocks.Infrastructure.Pipe
{
    using Sockets;
    public abstract class PipeFilter : IPipeFilter, IComparer<PipeFilter>
    {
        /// <summary>
        /// The client applies to.
        /// </summary>
        public IClient Client { get; protected set; }

        /// <summary>
        /// Smaller value higher priority.
        /// </summary>
        public byte Priority { get; protected set; }//TODO PipeFilterCategory


        protected ILogger _logger = null;


        /// <summary>
        /// Create a filter with a client and priority.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="priority"></param>
        public PipeFilter(IClient client, byte priority, ILogger logger = null)
        {
            Client = Throw.IfNull(() => client);
            Priority = priority;

            _logger = logger;
        }

        public abstract PipeFilterResult AfterReading(PipeFilterContext filterContext);
        public abstract PipeFilterResult BeforeWriting(PipeFilterContext filterContext);


        public int Compare(PipeFilter x, PipeFilter y)
        {
            return -x.Priority.CompareTo(y.Priority);
        }
    }
}
