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


    /// <summary>
    /// A duplex pipe that connects two clients and exchanges their data.
    /// </summary>
    public abstract class DuplexPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion

        public virtual IClient ClientA { get; protected set; }
        public virtual IClient ClientB { get; protected set; }

        public abstract IClientReaderAccessor Reader { get; }
        public abstract IClientWriterAccessor Writer { get; }


        protected ILogger _logger = null;


        /// <summary>
        /// Create a pipe with two clients.
        /// </summary>
        /// <param name="clientA"></param>
        /// <param name="clientB"></param>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public DuplexPipe(IClient clientA, IClient clientB)
        {
            ClientA = Throw.IfNull(() => clientA);
            ClientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => clientA, clientB);
        }

        /// <summary>
        /// Create a pipe with a pair of ClientReaders / ClientWriters.
        /// </summary>
        /// <param name="readerA"></param>
        /// <param name="writerA"></param>
        /// <param name="readerB"></param>
        /// <param name="writerB"></param>
        /// <param name="logger"></param>
        public DuplexPipe(IClientReader readerA, IClientWriter writerA, IClientReader readerB, IClientWriter writerB)
        {
            Throw.IfNull(() => readerA);
            Throw.IfNull(() => writerA);
            Throw.IfNotEqualsTo(() => readerA.Client, writerA.Client);

            Throw.IfNull(() => readerB);
            Throw.IfNull(() => writerB);
            Throw.IfNotEqualsTo(() => readerB.Client, writerB.Client);

            Throw.IfEqualsTo(() => readerA.Client, readerB.Client);
            Throw.IfEqualsTo(() => writerA.Client, writerB.Client);

            ClientA = readerA.Client;
            ClientB = readerB.Client;

        }


        /// <summary>
        /// Create an empty pipe and link the client later.
        /// </summary>
        /// <param name="bufferSize"></param>
        /// <param name="logger"></param>
        public DuplexPipe() { }

        ~DuplexPipe()
        {
            UnPipe();
        }

        public abstract void LinkupClientA(IClient client);
        public abstract void LinkupClientB(IClient client);

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
