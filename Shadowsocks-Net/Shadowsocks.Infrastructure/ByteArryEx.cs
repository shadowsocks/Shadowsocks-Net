/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure
{
    public static class ByteArryEx
    {
        public static string ToHexString(this byte[] arr)
        {
            return BitConverter.ToString(arr).Replace("-", "");
        }

    }
}
