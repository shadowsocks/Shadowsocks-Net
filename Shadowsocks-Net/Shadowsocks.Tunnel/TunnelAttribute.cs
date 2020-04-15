/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using Argument.Check;

namespace Shadowsocks.Tunnel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TunnelAttribute : Attribute
    {
        public string Name { set; get; }

        public TunnelAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }
            Name = name;
        }

    }
}
