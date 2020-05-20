/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Shadowsocks.Obfuscation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ObfuscatorAttribute : Attribute
    {
        public string Name { set; get; }
        public ObfuscatorAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }
            this.Name = name;
        }
    }
}
