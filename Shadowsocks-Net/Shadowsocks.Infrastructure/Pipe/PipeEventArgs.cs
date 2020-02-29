/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    public class PipeEventArgs : EventArgs
    {
        public IPipe Pipe { set; get; }
        public PipeException Exception { set; get; }

    }
    public class PipeBrokenEventArgs : PipeEventArgs
    {
        public PipeBrokenCause Cause { set; get; }
    }

    public enum PipeBrokenCause
    {
        Empty,
        Cancelled,
        FilterBreak,
        Exception
    }

    public struct PipingEventArgs
    {
        public IPEndPoint Origin;
        public IPEndPoint Destination;
        public int Bytes;
    }
}
