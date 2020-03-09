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
    /// ClientFilter processes data read from and write to clients.
    /// Note: Except for processing data, there should not be too much logic here.
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
        /// When sorting filters, categories will be considered first, then priority. see <see cref="Compare(ClientFilter, ClientFilter)"/>.
        /// </summary>
        public byte Priority { get; protected set; }


        protected ILogger _logger = null;

        /// <summary>
        /// Create a filter with a client, category and priority.
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

        public int Compare(IClientWriterFilter x, IClientWriterFilter y)
        {
            return this.Compare(x as ClientFilter, y as ClientFilter);
        }
    }
}
