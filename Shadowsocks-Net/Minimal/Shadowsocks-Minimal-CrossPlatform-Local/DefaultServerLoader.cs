/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Argument.Check;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadowsocks_Minimal_Crossplatform_Local
{
    using Shadowsocks;
    using Shadowsocks.Local;
    using System.Net;
    using System.Text.Encodings.Web;
    using System.Text.Unicode;

    public class DefaultServerLoader : IServerLoader
    {
        List<Server> _servers = null;
        object _readwriteLock = new object();

        public DefaultServerLoader()
        {
        }

        public Server Load(EndPoint target = null)
        {
            if (null == _servers || 0 == _servers.Count)
            {
                lock (_readwriteLock)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        //IgnoreReadOnlyProperties = true,
                        //IgnoreNullValues = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        //PropertyNameCaseInsensitive = false;
                        //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)//, (UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    };
                    //options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    try
                    {
                        _servers = JsonSerializer.Deserialize<List<Server>>(
                            File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "servers.json")), options);
                    }
                    catch { return null; }
                }
            }
            if (_servers.Count > 0)
            {
                return _servers[0];
            }
            return null;
        }
    }
}
