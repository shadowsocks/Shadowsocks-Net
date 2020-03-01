/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Argument.Check;

namespace Shadowsocks
{
    using Infrastructure;
    using Infrastructure.Sockets;

    /*
        +------+----------+----------+
        | ATYP | BND.ADDR | BND.PORT |
        +------+----------+----------+
        |  1   | Variable |    2     |
        +------+----------+----------+

        [1-byte type][variable-length host][2-byte port]

        The following address types are defined:
            0x01: host is a 4-byte IPv4 address.
            0x03: host is a variable length string, starting with a 1-byte length, followed by up to 255-byte domain name.
            0x04: host is a 16-byte IPv6 address.
            The port number is a 2-byte big-endian unsigned integer.
     */
    public struct ShadowsocksAddress
    {
        /// <summary>
        /// 0x01=IPv4, 0x3=Host string, 0x4=IPv6
        /// </summary>
        public byte ATYP;

        //ushort
        public ushort Port;

        /// <summary>
        /// Pure address data.
        /// </summary>
        public ReadOnlyMemory<byte> Address;

        /// <summary>
        /// The slice of raw memory, which contains ATYP, BND.ADDR and BND.PORT exactly.
        /// </summary>
        public ReadOnlyMemory<byte> RawMemory;

        public static bool TryResolve(ReadOnlyMemory<byte> raw, out ShadowsocksAddress ssAddr)
        {
            ssAddr = default;
            if (raw.Length < 5) { return false; }

            ShadowsocksAddress addr;
            addr.ATYP = raw.Span[0];

            if (0x1 == addr.ATYP)
            {
                if (7 > raw.Length) { return false; }
                addr.Address = raw.Slice(1, 4);
                addr.RawMemory = raw.Slice(0, 7);
                BinaryPrimitives.TryReadUInt16BigEndian(raw.Span.Slice(5, 2), out addr.Port);
                ssAddr = addr;
                return true;
            }
            else if (0x4 == addr.ATYP)
            {
                if (19 > raw.Length) { return false; }
                addr.Address = raw.Slice(1, 16);
                addr.RawMemory = raw.Slice(0, 19);
                BinaryPrimitives.TryReadUInt16BigEndian(raw.Span.Slice(17, 2), out addr.Port);
                ssAddr = addr;
                return true;
            }
            else if (0x3 == addr.ATYP)
            {
                byte len = raw.Span[1];
                if (4 + len > raw.Length) { return false; }
                addr.Address = raw.Slice(2, len);
                addr.RawMemory = raw.Slice(0, 4 + len);
                BinaryPrimitives.TryReadUInt16BigEndian(raw.Span.Slice(1 + 1 + len, 2), out addr.Port);
                ssAddr = addr;
                return true;
            }
            return false;
        }

        public static bool TryResolveLength(ReadOnlyMemory<byte> raw, out int length)
        {
            length = 0;
            if (raw.Length < 5) { return false; }
            byte atyp = raw.Span[0];
            if (0x1 == atyp) { length = 7; return raw.Length >= 7; }
            else if (0x4 == atyp) { length = 19; return raw.Length >= 19; }
            else if (0x3 == atyp)
            {
                length = 4 + raw.Span[1];
                return raw.Length >= (4 + raw.Span[1]);
            }
            return false;
        }

        public static byte[] TrySerialize(byte ATYP, byte[] address, ushort port)
        {
            byte[] ssAddr = null;
            if (0x1 == ATYP)
            {
                if (address.Length != 4) { return null; }
                ssAddr = new byte[7];
                ssAddr[0] = ATYP;
                address.CopyTo(ssAddr, 1);
                BinaryPrimitives.TryWriteUInt16BigEndian(ssAddr.AsSpan().Slice(5, 2), port);
                return ssAddr;
            }
            else if (0x4 == ATYP)
            {
                if (address.Length != 16) { return null; }
                ssAddr = new byte[19];
                ssAddr[0] = ATYP;
                address.CopyTo(ssAddr, 1);
                BinaryPrimitives.TryWriteUInt16BigEndian(ssAddr.AsSpan().Slice(17, 2), port);
                return ssAddr;
            }
            else if (0x3 == ATYP)
            {
                if (address.Length > byte.MaxValue) { return null; }
                ssAddr = new byte[4 + address.Length];
                ssAddr[0] = ATYP;
                ssAddr[1] = (byte)address.Length;
                address.CopyTo(ssAddr, 2);
                BinaryPrimitives.TryWriteUInt16BigEndian(ssAddr.AsSpan().Slice(ssAddr.Length - 2, 2), port);
                return ssAddr;
            }
            return null;
        }

        public static bool TrySerailizeTo(byte ATYP, byte[] address, ushort port, Memory<byte> memory, out int written)
        {
            written = 0;
            if (0x1 == ATYP)
            {
                if (address.Length != 4 || 7 > memory.Length) { return false; }
                written = 7;
                memory.Span[0] = ATYP;
                address.CopyTo(memory.Slice(1));
                return BinaryPrimitives.TryWriteUInt16BigEndian(memory.Span.Slice(5, 2), port);
            }
            else if (0x4 == ATYP)
            {
                if (address.Length != 16 || 19 > memory.Length) { return false; }
                written = 19;
                memory.Span[0] = ATYP;
                address.CopyTo(memory.Slice(1));
                return BinaryPrimitives.TryWriteUInt16BigEndian(memory.Span.Slice(17, 2), port);
            }
            else if (0x3 == ATYP)
            {
                if (address.Length > byte.MaxValue || 4 + address.Length > memory.Length) { return false; }
                written = 4 + address.Length;
                memory.Span[0] = ATYP;
                memory.Span[1] = (byte)address.Length;
                address.CopyTo(memory.Slice(2));
                return BinaryPrimitives.TryWriteUInt16BigEndian(memory.Span.Slice(1 + 1 + address.Length, 2), port);
            }
            return false;
        }

        public static bool TryParse(Uri uri, out Tuple<byte, ushort, byte[]> shadowsocksAddress)
        {
            shadowsocksAddress = default;

            byte ATYP = 0;
            ushort port = (ushort)uri.Port;
            byte[] addrss = null;

            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    {
                        ATYP = 0x3;
                        addrss = Encoding.ASCII.GetBytes(uri.DnsSafeHost);
                        if (addrss.Length > 255) { return false; }
                    }
                    break;
                case UriHostNameType.IPv4:
                    {
                        ATYP = 0x1;
                        if (IPAddress.TryParse(uri.DnsSafeHost, out IPAddress ip))
                        {
                            addrss = ip.GetAddressBytes();
                        }
                        else { return false; }
                    }
                    break;
                case UriHostNameType.IPv6:
                    {
                        ATYP = 0x4;
                        if (IPAddress.TryParse(uri.DnsSafeHost, out IPAddress ip))
                        {
                            addrss = ip.GetAddressBytes();
                        }
                        else { return false; }
                    }
                    break;
                case UriHostNameType.Basic:
                case UriHostNameType.Unknown:
                default:
                    ATYP = 0x0;
                    break;
            }

            if (0x0 == ATYP) { return false; }
           
            shadowsocksAddress = Tuple.Create<byte, ushort, byte[]>(ATYP, port, addrss);
            return true;

        }
    }
}
