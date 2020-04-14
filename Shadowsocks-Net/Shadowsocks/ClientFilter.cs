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

namespace Shadowsocks
{
    using Infrastructure;
    using Infrastructure.Pipe;
    using Infrastructure.Sockets;

    /// <summary>
    /// ClientFilter processes data read from and write to clients.
    /// Note: Except for processing data, there should not be too much logic here.
    /// </summary>
    public abstract class ClientFilter : IClientFilter
    {
        /// <summary>
        /// The client applies to.
        /// </summary>
        public IClient Client { get; set; }

        /// <summary>
        /// Filter category.
        /// </summary>
        public ClientFilterCategory Category { get; protected set; }

        /// <summary>
        /// Smaller value higher priority.
        /// When sorting filters, categories will be considered first, then priority. see <see cref="Compare(IFilter, IFilter)"/>.
        /// </summary>
        public byte Priority { get; protected set; }


        protected ILogger _logger = null;

        /// <summary>
        /// Create a filter.
        /// </summary>        
        /// <param name="category"></param>
        /// <param name="priority"></param>
        public ClientFilter(ClientFilterCategory category, byte priority, ILogger logger = null)
        {
            Category = category;
            Priority = priority;

            _logger = logger;
        }

        public abstract ClientFilterResult OnReading(ClientFilterContext filterContext);
        public abstract ClientFilterResult OnWriting(ClientFilterContext filterContext);

        public int Compare(IFilter x, IFilter y)
        {
            var a = x as ClientFilter;
            var b = y as ClientFilter;

            int r = a.Category.CompareTo(b.Category);
            return r != 0 ? r : a.Priority.CompareTo(b.Priority);
        }
    }
}
