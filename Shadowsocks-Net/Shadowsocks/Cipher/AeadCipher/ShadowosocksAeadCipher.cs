/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Argument.Check;


namespace Shadowsocks.Cipher.AeadCipher
{
    using Infrastructure;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;

    /// <summary>
    /// Not thread-safe.
    /// </summary>
    public abstract class ShadowosocksAeadCipher : ShadowsocksCipher, IShadowsocksAeadCipher
    {
        //Shadowsocks - AEAD Ciphers  https://shadowsocks.org/en/spec/AEAD-Ciphers.html



        protected const int LEN_NONCE = 12;
        protected const int LEN_TAG = 16;
        protected static readonly byte[] SubkeyInfoBytes = Encoding.ASCII.GetBytes("ss-subkey");
        protected static readonly byte[] NonceZero = new byte[LEN_NONCE];

        protected readonly ValueTuple<int, int> _keySize_SaltSize = default;

        protected readonly HkdfBytesGenerator _hkdf = null;
        protected readonly byte[] _masterKeyBytes = null;

        #region TCP
        public const int LEN_TCP_PAYLOADLEN_LENGTH = 2;//
        public const int LEN_TCP_OVERHEAD_PER_CHUNK = LEN_TAG * 2 + LEN_TCP_PAYLOADLEN_LENGTH;//
        public const int LEN_TCP_MAX_CHUNK = 16 * 1024 - 1;//20;//

        TcpCipherContext _tcpCtx加密 = default;
        TcpCipherContext _tcpCtx解密 = default;

        SmartBuffer _tcp_decrypt_crumb = null;
        #endregion

        protected ILogger _logger = null;


        public ShadowosocksAeadCipher(string password, ValueTuple<int, int> key_salt_size, ILogger logger = null)
            : base(password)
        {
            _keySize_SaltSize = key_salt_size;
            _logger = logger;

            _hkdf = new HkdfBytesGenerator(new Sha1Digest());
            _masterKeyBytes = MasterKeyGenerator.PasswordToKey(password, _keySize_SaltSize.Item1);

            _logger?.LogInformation($"ShadowosocksAeadCipher ctor " +
                $"_keySize_SaltSize={_keySize_SaltSize.Item1},{_keySize_SaltSize.Item2}");
            _tcpCtx加密 = new TcpCipherContext(_keySize_SaltSize.Item1, _keySize_SaltSize.Item2);
            _tcpCtx解密 = new TcpCipherContext(_keySize_SaltSize.Item1, _keySize_SaltSize.Item2);

            
        }

        ~ShadowosocksAeadCipher()
        {
            _tcp_decrypt_crumb?.Dispose();
        }


        public virtual SmartBuffer EncryptTcp(ReadOnlyMemory<byte> plain)
        {
            if (plain.IsEmpty) { _logger?.LogInformation($"ShadowosocksAeadCipher EncryptTcp plain.IsEmpty."); return null; }
            int cipherLen = CalcTcpCipherStreamLength(plain.Length);
            SmartBuffer cihperBuffer = SmartBuffer.Rent(cipherLen);

            var cipherStream = new MemoryWriter(cihperBuffer.Memory);

            if (!_tcpCtx加密.HaveSalt())
            {
                _tcpCtx加密.NewSalt();

                cipherStream.Write(_tcpCtx加密.Salt.AsMemory());
                _logger?.LogInformation($"ShadowosocksAeadCipher EncryptTcp new salt wrote. { _tcpCtx加密.Salt.ToHexString()}");
                DeriveSubKey(_masterKeyBytes, _tcpCtx加密.Salt, SubkeyInfoBytes, _tcpCtx加密.Key, _tcpCtx加密.Key.Length);
                _logger?.LogInformation($"ShadowosocksAeadCipher EncryptTcp new subkey {_tcpCtx加密.Key.ToHexString()}.");
            }

            int remain = plain.Length;
            int chunkLen = 0, encrypted = 0;
            do
            {
                chunkLen = Math.Min(remain, LEN_TCP_MAX_CHUNK);

                //payload length.                                   
                {
                    byte[] payloadLenBytes = new byte[2];//ushort
                    BinaryPrimitives.TryWriteUInt16BigEndian(payloadLenBytes, (ushort)chunkLen);
                    //byte[] payloadLenBytes = BitConverter.GetBytes((ushort)(System.Net.IPAddress.HostToNetworkOrder((short)chunkLen)));
                    _logger?.LogInformation($"ShadowosocksAeadCipher EncryptTcp  chunklen={chunkLen}, payloadLenBytes={payloadLenBytes.ToHexString()}.");
                    using (var payloadLenC = this.EncryptChunk(payloadLenBytes, _tcpCtx加密.Key, _tcpCtx加密.Nonce))
                    {
                        //[encrypted payload length][length tag]
                        cipherStream.Write(payloadLenC.SignificantMemory);
                        _tcpCtx加密.IncreaseNonce();
                    }
                }
                //payload.               
                {
                    var payload = plain.Slice(plain.Length - remain, chunkLen);
                    using (var payloadC = this.EncryptChunk(payload, _tcpCtx加密.Key, _tcpCtx加密.Nonce))
                    {
                        //[encrypted payload][payload tag]
                        cipherStream.Write(payloadC.SignificantMemory);
                        _tcpCtx加密.IncreaseNonce();
                    }
                }
                //-------------------------------
                encrypted += chunkLen;
                _logger?.LogInformation($"ShadowosocksAeadCipher EncryptTcp encrypted {encrypted} bytes.");
                remain -= chunkLen;
            } while (remain > 0);


            cihperBuffer.SignificantLength = cipherStream.Position;
            return cihperBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns>plain. null if decrypt failed.</returns>
        public virtual SmartBuffer DecryptTcp(ReadOnlyMemory<byte> cipher)
        {
            if (cipher.IsEmpty) { _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp plain.IsEmpty."); return null; }
            bool decrypteFailed = false;
            #region crumb
            if (null !=_tcp_decrypt_crumb && _tcp_decrypt_crumb.SignificantLength > 0)//have crumb left lasttime.
            {
                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp crumb {_tcp_decrypt_crumb.SignificantLength} bytes found.");
                using (var combCipher = SmartBuffer.Rent(cipher.Length + _tcp_decrypt_crumb.SignificantLength))
                {
                    //combine crumb
                    var combCipherStream = new MemoryWriter(combCipher.Memory);
                    combCipherStream.Write(_tcp_decrypt_crumb.SignificantMemory);
                    combCipherStream.Write(cipher);
                    combCipher.SignificantLength = cipher.Length + _tcp_decrypt_crumb.SignificantLength;
                    _tcp_decrypt_crumb.SignificantLength = 0;
                    _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp combined crumb + new cipher = {combCipher.SignificantLength} bytes.");
                    return DecryptTcp(combCipher.SignificantMemory);
                }
            }
            if (cipher.Length <= LEN_TCP_OVERHEAD_PER_CHUNK)//still an incomplete chunk.
            {
                Initialize_TcpDecrypt_Crumb();
                cipher.CopyTo(_tcp_decrypt_crumb.Memory);
                _tcp_decrypt_crumb.SignificantLength = cipher.Length;
                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp save crumb {_tcp_decrypt_crumb.SignificantLength} bytes.");
                return null;
            }
            #endregion

            var cipherStreamReader = new ReadOnlyMemoryReader(cipher);

            if (!_tcpCtx解密.HaveSalt())
            {
                _tcpCtx解密.SetSalt(cipherStreamReader.Read(_tcpCtx解密.Salt.Length));
                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp salt read. {_tcpCtx解密.Salt.ToHexString()}");
                DeriveSubKey(_masterKeyBytes, _tcpCtx解密.Salt, SubkeyInfoBytes, _tcpCtx解密.Key, _tcpCtx解密.Key.Length);
                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp new subkey. {_tcpCtx解密.Key.ToHexString()}");
            }

            var plainBuffer = SmartBuffer.Rent(cipher.Length);//enough            
            var plainStream = new MemoryWriter(plainBuffer.Memory);

            int remain = cipher.Length;

            do
            {
                const int LEN_AND_LENTAG = LEN_TCP_PAYLOADLEN_LENGTH + LEN_TAG;
                ushort payloadLen = 0;
                //payloadlen
                {
                    var arrPayloadLen = cipherStreamReader.Read(LEN_AND_LENTAG);

                    using (var len = this.DecryptChunk(arrPayloadLen, _tcpCtx解密.Key, _tcpCtx解密.Nonce))
                    {
                        _tcpCtx解密.IncreaseNonce();
                        if (0 >= len.SignificantLength)
                        {
                            decrypteFailed = true;//TODO close client or not
                            break;
                        }

                        var payloadLenBytes = len.SignificantMemory;
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp payloadLenBytes ={payloadLenBytes.ToArray().ToHexString()}.");
                        // payloadLen = (ushort)System.Net.IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(payloadLenBytes.Span));
                        BinaryPrimitives.TryReadUInt16BigEndian(payloadLenBytes.Span, out payloadLen);
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp decrypted payloadLen={payloadLen}.");
                    }
                    if (payloadLen <= 0)
                    {
                        decrypteFailed = true;
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp something weird happened.");
                        break;
                    }
                    if (!decrypteFailed && payloadLen + LEN_TAG > (cipherStreamReader.Length - cipherStreamReader.Position))
                    {
                        Initialize_TcpDecrypt_Crumb();

                        cipher.Slice(cipherStreamReader.Position - LEN_AND_LENTAG).CopyTo(_tcp_decrypt_crumb.Memory);
                        _tcp_decrypt_crumb.SignificantLength = (cipherStreamReader.Length - cipherStreamReader.Position + LEN_AND_LENTAG);
                        remain = 0;
                        _tcpCtx解密.DecreaseNonce();
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp save incomplete chunk {_tcp_decrypt_crumb.SignificantLength} bytes.");
                        break;
                    }
                }
                //payload
                {
                    var arrPayload = cipherStreamReader.Read(payloadLen + LEN_TAG);
                    using (var payload = this.DecryptChunk(arrPayload, _tcpCtx解密.Key, _tcpCtx解密.Nonce))
                    {
                        _tcpCtx解密.IncreaseNonce();
                        if (0 >= payload.SignificantLength)
                        {
                            decrypteFailed = true;
                            break;
                        }
                        plainStream.Write(payload.SignificantMemory);

                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp decrypted payload {payload.SignificantLength} bytes.");
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp decrypted plain= " +
                            $"{payload.SignificantMemory.ToArray().ToHexString()}");
                        _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp decrypted plain.UTF8= " +
                           $"{Encoding.UTF8.GetString( payload.SignificantMemory.ToArray())}\r\n");
                    }
                }
                //-----
                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp one chunk decrypted.");
                remain = cipherStreamReader.Length - cipherStreamReader.Position;
            } while (remain > LEN_TCP_OVERHEAD_PER_CHUNK);

            if (!decrypteFailed && 0 < remain && remain <= LEN_TCP_OVERHEAD_PER_CHUNK)//crumb
            {
                Initialize_TcpDecrypt_Crumb();

                _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp save incomplete chunk {_tcp_decrypt_crumb.SignificantLength} bytes.");
                cipher.Slice(cipher.Length - remain, remain).CopyTo(_tcp_decrypt_crumb.Memory);
                _tcp_decrypt_crumb.SignificantLength += remain;//_decrypt_crumb must be empty. //TODO not thread-safe.
            }
            if (decrypteFailed && null != _tcp_decrypt_crumb) { _tcp_decrypt_crumb.Erase(); }
            plainBuffer.SignificantLength = decrypteFailed ? 0 : plainStream.Position;
            _logger?.LogInformation($"ShadowosocksAeadCipher DecryptTcp {plainBuffer.SignificantLength} bytes got, will return.");
            return plainBuffer;
        }


        public virtual SmartBuffer EncryptUdp(ReadOnlyMemory<byte> plain)
        {
            if (plain.IsEmpty) { _logger?.LogInformation($"ShadowosocksAeadCipher EncryptUdp plain.IsEmpty."); return null; }

            var cipherPacket = SmartBuffer.Rent(1500 + LEN_TAG + _keySize_SaltSize.Item2);
            var cipherPacketStream = new MemoryWriter(cipherPacket.Memory);

            byte[] salt = new byte[_keySize_SaltSize.Item2], key = new byte[_keySize_SaltSize.Item1];
            RandomNumberGenerator.Fill(salt);
            DeriveSubKey(_masterKeyBytes, salt, SubkeyInfoBytes, key, key.Length);


            //[encrypted payload][tag]
            using (var payload = this.EncryptChunk(plain, key.AsSpan(), NonceZero.AsSpan()))
            {
                cipherPacketStream.Write(salt.AsMemory());
                cipherPacketStream.Write(payload.SignificantMemory);
                cipherPacket.SignificantLength = cipherPacketStream.Position;
                //[salt][encrypted payload][tag]
            }

            return cipherPacket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipher">[salt][encrypted payload][tag]</param>
        /// <returns>[plain]. null if decrypt failed.</returns>
        public virtual SmartBuffer DecryptUdp(ReadOnlyMemory<byte> cipher)
        {
            if (cipher.IsEmpty || cipher.Length <= (LEN_TAG + _keySize_SaltSize.Item2))
            { _logger?.LogInformation($"ShadowosocksAeadCipher DecryptUdp plain.IsEmpty."); return null; }

            byte[] key = new byte[_keySize_SaltSize.Item1];
            DeriveSubKey(_masterKeyBytes, cipher.Slice(0, _keySize_SaltSize.Item2).ToArray(), SubkeyInfoBytes, key, key.Length);

            return this.DecryptChunk(cipher.Slice(_keySize_SaltSize.Item2), key.AsSpan(), NonceZero.AsSpan());
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw">[Plain]</param>
        /// <param name="key"></param>
        /// <param name="nonce"></param>
        /// <param name="add"></param>
        /// <returns>[Cipher][Tag]</returns>
        protected abstract SmartBuffer EncryptChunk(ReadOnlyMemory<byte> raw, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> add = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipher">[Cipher][Tag]</param>
        /// <param name="key"></param>
        /// <param name="nonce"></param>
        /// <param name="add"></param>
        /// <returns>[Plain]</returns>
        protected abstract SmartBuffer DecryptChunk(ReadOnlyMemory<byte> cipher, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> add = default);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int CalcTcpCipherStreamLength(int rawLen)
        {
            int chunkCount = (int)Math.Ceiling((double)rawLen / (double)LEN_TCP_MAX_CHUNK);
            return rawLen + chunkCount * LEN_TCP_OVERHEAD_PER_CHUNK + _keySize_SaltSize.Item2;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DeriveSubKey(byte[] materKey, byte[] salt, byte[] info, byte[] subkeyBuffer, int subKeyLength)
        {
            //Throw.IfNull(() => materKey);
            //Throw.IfNull(() => salt);
            //Throw.IfNull(() => info);

            _hkdf.Init(new HkdfParameters(materKey, salt, info));
            _hkdf.GenerateBytes(subkeyBuffer, 0, subKeyLength);
            //TODO hkdf .NET Core 5.0 //HKDF implementation by krwq · Pull Request #42567 · dotnet/corefx  https://github.com/dotnet/corefx/pull/42567
            //https://gist.github.com/charlesportwoodii/09ffd6868c2a6e55826c4d5ebb509651
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Initialize_TcpDecrypt_Crumb()
        {
            if (null == _tcp_decrypt_crumb) { _tcp_decrypt_crumb = SmartBuffer.Rent(LEN_TCP_MAX_CHUNK + LEN_TCP_OVERHEAD_PER_CHUNK + 100); }
        }

        public struct TcpCipherContext
        {
            public ulong NonceValue;
            public readonly byte[] Nonce;
            public readonly byte[] Key;
            public readonly byte[] Salt;
            private bool _haveSalt;
            //public readonly byte[] PayloadLenCipherBuffer;

            public TcpCipherContext(int keyLen, int saltLen)
            {
                NonceValue = 0U;
                Nonce = new byte[LEN_NONCE];
                BinaryPrimitives.TryWriteUInt64LittleEndian(Nonce, NonceValue);

                Key = new byte[keyLen];
                Salt = new byte[saltLen];
                _haveSalt = false;

                //PayloadLenCipherBuffer = new byte[SIZE_TCP_PAYLOAD_LEN + SIZE_TAG];
            }
            public bool HaveSalt()
            {
                return _haveSalt;
            }

            public void NewSalt()
            {
                RandomNumberGenerator.Fill(Salt);
                _haveSalt = true;
            }

            public void SetSalt(ReadOnlyMemory<byte> salt)
            {
                salt.CopyTo(Salt);
                _haveSalt = true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void IncreaseNonce()
            {
                ++NonceValue;
                BinaryPrimitives.TryWriteUInt64LittleEndian(Nonce, NonceValue);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DecreaseNonce()
            {
                --NonceValue;
                BinaryPrimitives.TryWriteUInt64LittleEndian(Nonce, NonceValue);
            }
          
        }

    }
}
