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

    /// <summary>
    /// 1.Don't put too much logic in ClientFilter, although you can do it.
    /// 2.Don't do too much except process data.
    /// 3.Always copy data.
    /// </summary>
    public abstract class ClientFilter : IClientReaderFilter, IClientWriterFilter, IComparer<ClientFilter>
    {
        /// <summary>
        /// The client applies to.
        /// </summary>
        public IClient Client { get; protected set; }

        /// <summary>
        /// Filter category.
        /// </summary>
        public ClientFilterCategory Category { get; protected set; }

        /// <summary>
        /// Smaller value higher priority.
        /// </summary>
        public byte Priority { get; protected set; }


        protected ILogger _logger = null;

        /// <summary>
        /// Create a filter with a client, catetory and priority.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="category"></param>
        /// <param name="priority"></param>
        public ClientFilter(IClient client, ClientFilterCategory category, byte priority)
        {
            Client = Throw.IfNull(() => client);
            Category = category;
            Priority = priority;
        }

        public abstract ClientFilterResult AfterReading(ClientFilterContext filterContext);
        public abstract ClientFilterResult BeforeWriting(ClientFilterContext filterContext);


        public int Compare(ClientFilter x, ClientFilter y)
        {
            int c = (int)x.Category.CompareTo((int)y.Category);
            return c != 0 ? c : x.Priority.CompareTo(y.Priority);
        }
    }
}
