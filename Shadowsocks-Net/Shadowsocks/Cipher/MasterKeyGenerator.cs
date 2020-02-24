/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Buffers;
using System.Linq;
using System.Text;
using Argument.Check;


namespace Shadowsocks.Cipher
{
    using Infrastructure;
    public static class MasterKeyGenerator
    {
        static MD5 md5 = null;
        static MasterKeyGenerator()
        {
            md5 ??= MD5.Create();
        }

        public static byte[] PasswordToKey(string password, int keyLength)
        {
            Throw.IfNullOrEmpty(() => password);
            Throw.IfLessThan(() => keyLength, 16);//MD5 len=16

            byte[] key = new byte[keyLength];

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            ////////using (MD5 md5 = MD5.Create())
            {
                var passwordHash = md5.ComputeHash(passwordBytes);//16
                Buffer.BlockCopy(passwordHash, 0, key, 0, passwordHash.Length);

                int keyOffset = passwordHash.Length;
                while (keyOffset < key.Length)
                {
                    var temp = passwordHash.Concat(passwordBytes).ToArray();
                    passwordHash = md5.ComputeHash(temp);
                    Buffer.BlockCopy(passwordHash, 0, key, keyOffset, Math.Min((key.Length - keyOffset), passwordHash.Length));
                    keyOffset += passwordHash.Length;
                }
            }
            return key;
        }

    }
}
