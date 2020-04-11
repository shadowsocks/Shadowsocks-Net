/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections;
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

    public abstract class DuplexPipeX : IDuplexPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion

        /// <summary>
        /// Get or set the pipe's client A.
        /// </summary>
        public virtual IClient ClientA { get; set; }

        /// <summary>
        /// Get or set the pipe's client B.
        /// </summary>
        public virtual IClient ClientB { get; set; }


        /// <summary>
        /// Get the pipe's client Reader by client.
        /// </summary>
        public abstract IClientReaderAccessor Reader { get; }

        /// <summary>
        /// Get the pipe's client Writer by client.
        /// </summary>
        public abstract IClientReaderAccessor Writer { get; }


        protected ILogger _logger = null;


        /// <summary>
        /// Create a pipe with two clients.
        /// </summary>
        /// <param name="clientA"></param>
        /// <param name="clientB"></param>
        public DuplexPipeX(IClient clientA, IClient clientB) { }

        /// <summary>
        /// Create a pipe with a pair of ClientReaders / ClientWriters.
        /// </summary>
        /// <param name="readerA"></param>
        /// <param name="writerA"></param>
        /// <param name="readerB"></param>
        /// <param name="writerB"></param>
        public DuplexPipeX(IReader readerA, IWriter writerA, IReader readerB, IWriter writerB) { }


        /// <summary>
        /// Create an empty pipe and link the clients later.
        /// </summary>
        public DuplexPipeX() { }

        ~DuplexPipeX()
        {
            StopPipe();
        }

        public abstract void StartPipe(CancellationToken cancellationToken);
        public abstract void StopPipe();

        protected virtual void ReportBroken(PipeBrokenCause cause, PipeException exception = null)
        {
            try
            {
                if (null != OnBroken)
                {
                    OnBroken(this, new PipeBrokenEventArgs()
                    {
                        Pipe = this,
                        Cause = cause,
                        Exception = exception
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Pipe ReportBroken error.");
            }
        }
        protected virtual void ReportPiping(PipingEventArgs pipingEventArgs)
        {
            try
            {
                if (null != OnPiping)
                {
                    OnPiping(this, pipingEventArgs);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Pipe ReportPiping error.");
            }
        }
    }
}
