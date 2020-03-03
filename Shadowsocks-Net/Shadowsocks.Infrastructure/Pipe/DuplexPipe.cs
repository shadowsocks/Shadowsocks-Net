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
    /// The pipe.
    /// </summary>
    public abstract class DuplexPipe
    {
        #region Events
        public event EventHandler<PipeBrokenEventArgs> OnBroken;
        public event EventHandler<PipingEventArgs> OnPiping;
        #endregion

        public IClient ClientA { get; protected set; }
        public IClient ClientB { get; protected set; }

        protected ILogger _logger = null;

        public DuplexPipe(IClient clientA, IClient clientB)
        {
            ClientA = Throw.IfNull(() => clientA);
            ClientB = Throw.IfNull(() => clientB);
            Throw.IfEqualsTo(() => clientA, clientB);
        }

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
