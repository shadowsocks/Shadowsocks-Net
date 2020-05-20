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
        public static string ToUTF8String(this byte[] arr)
        {
            return Encoding.UTF8.GetString(arr);
        }
        public static string ToASCIIString(this byte[] arr)
        {
            return Encoding.ASCII.GetString(arr);
        }
    }
}
