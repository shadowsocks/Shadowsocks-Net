/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using Argument.Check;

namespace Shadowsocks.Cipher
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CipherAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public CipherAttribute(string name, bool isDefault = false)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }
            Name = name;
            IsDefault = isDefault;
        }

    }
}
