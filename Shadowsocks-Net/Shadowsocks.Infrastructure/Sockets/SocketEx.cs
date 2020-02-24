/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Shadowsocks.Infrastructure
{
    public static class SocketEx
    {
        public static void KeepAlive(this Socket socket, int onOff, int keepAliveTime, int keepAliveInterval)
        {
            if (null != socket)
            {
                try
                {
                    byte[] buffer = new byte[12];
                    BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
                    BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
                    BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);

                    socket.IOControl(IOControlCode.KeepAliveValues, buffer, null);
                }
                catch { }
            }
        }

    }
}
