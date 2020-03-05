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
    /*
      +---------+   +----------+                       +----------+   +---------+ 
      |         |>>>| ReaderA  | >>>>>>>>>>>>>>>>>>>>> | WriterB  |>>>|         |
      | ClientA |   +----------+                       +----------+   | ClientB |
      |         |   +----------+                       +----------+   |         |
      |         |<<<| WriterA  | <<<<<<<<<<<<<<<<<<<<< | ReaderB  |<<<|         |
      +---------+   +----------+                       +----------+   +---------+                                                       
     */

    /// <summary>
    /// A duplex pipe that links two clients and exchanges their data.
    /// </summary>
    public abstract class DuplexPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion

        public virtual IClient ClientA { get; set; }
        public virtual IClient ClientB { get; set; }

        public abstract IClientReaderAccessor Reader { get; }
        public abstract IClientWriterAccessor Writer { get; }


        protected ILogger _logger = null;


        /// <summary>
        /// Create a pipe with two clients.
        /// </summary>
        /// <param name="clientA"></param>
        /// <param name="clientB"></param>
        public DuplexPipe(IClient clientA, IClient clientB) { }

        /// <summary>
        /// Create a pipe with a pair of ClientReaders / ClientWriters.
        /// </summary>
        /// <param name="readerA"></param>
        /// <param name="writerA"></param>
        /// <param name="readerB"></param>
        /// <param name="writerB"></param>
        public DuplexPipe(IClientReader readerA, IClientWriter writerA, IClientReader readerB, IClientWriter writerB) { }


        /// <summary>
        /// Create an empty pipe and link the client later.
        /// </summary>
        public DuplexPipe() { }

        ~DuplexPipe()
        {
            UnPipe();
        }

        public abstract void Pipe(CancellationToken cancellationToken);
        public abstract void UnPipe();

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
