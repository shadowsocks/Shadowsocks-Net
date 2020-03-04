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

namespace Shadowsocks.Http
{
    using Infrastructure;
    using Infrastructure.Sockets;

    public static class HttpProxyHeaderResolver
    {

        /*
        https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Methods
        https://tools.ietf.org/html/rfc7230#section-3.2
        https://tools.ietf.org/html/rfc7231#section-4        
        */
        public enum Verb
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        }

        public static bool TryResolve(ReadOnlyMemory<byte> proxyRequestHeader, out Verb httpVerb, out Uri targetHost, out byte[] normalRequestHeader)
        {
            httpVerb = Verb.GET;
            normalRequestHeader = default;
            targetHost = null;//

            if (proxyRequestHeader.IsEmpty || proxyRequestHeader.Length < 10) { return false; }

            var lines = Encoding.ASCII.GetString(proxyRequestHeader.Span).Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

            //Console.WriteLine("=============Proxy headers===============");
            //Console.WriteLine("" + string.Join("\r\n", lines));
            //Console.WriteLine("=========================================");


            List<string> newHeader = new List<string>();

            //line1
            {
                //VERB HOST HTTP/1.1            
                var line1 = lines[0];
                lines.Remove(line1);
                if (!line1.EndsWith("1.1") && !line1.EndsWith("1.0")) { return false; }

                var arr = line1.Split(' ');
                if (arr.Length < 3) { return false; }

                var verb = arr[0];
                var host = arr[1];//
                if (!Enum.TryParse(verb, true, out httpVerb)) { return false; }

                if (!host.ToLower().StartsWith("http://") && !host.ToLower().StartsWith("https://"))
                {
                    host = string.Format("https://{0}", host);
                }               

                if (Uri.TryCreate(host, UriKind.Absolute, out Uri hostUri)
                        && (hostUri.Scheme == Uri.UriSchemeHttp || hostUri.Scheme == Uri.UriSchemeHttps))
                {
                    targetHost = hostUri;                    

                    if (targetHost.DnsSafeHost.Length > 255) { return false; }//to long to adapt to socks5.

                    if (Verb.OPTIONS == httpVerb && string.IsNullOrEmpty(targetHost.PathAndQuery))
                    {
                        newHeader.Add($"OPTION * {arr[2]}\r\n");
                    }
                    else if (Verb.CONNECT == httpVerb) { }
                    else
                    {
                        newHeader.Add($"{verb} {targetHost.PathAndQuery} {arr[2]}\r\n");
                    }
                    newHeader.Add($"Host: {targetHost.Host}\r\n");
                }//invalid URL
                else { return false; }
            }
            if (null == targetHost) { return false; }

            //build new header
            foreach (var line in lines)
            {
                if (line.StartsWith("Host:", StringComparison.CurrentCultureIgnoreCase)) { continue; }
                else if (line.StartsWith("Proxy-Connection", StringComparison.CurrentCultureIgnoreCase))
                {
                    newHeader.Add("Connection:keep-alive\r\n");
                }
                else if (line.StartsWith("Proxy")) { continue; }
                else
                {
                    newHeader.Add(line);
                    newHeader.Add("\r\n");
                }
            }
            newHeader.Add("\r\n");

            normalRequestHeader = Encoding.ASCII.GetBytes(string.Join("", newHeader));
            return true;

        }
    }
}
