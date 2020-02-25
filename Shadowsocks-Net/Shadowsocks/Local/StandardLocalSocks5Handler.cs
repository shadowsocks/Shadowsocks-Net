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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Argument.Check;


namespace Shadowsocks.Local
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;

    sealed class StandardLocalSocks5Handler : ISocks5Handler
    {
        //accept->add
        //error->remove
        //expire->remove
        // ClientCollection<TcpClient1> _tcpClients = new ClientCollection<TcpClient1>();
        //TcpServer tcpServer

        //tcp lost->remove udp
        //pipe broken -> remove

        List<DefaultPipe> _pipes = new List<DefaultPipe>();

        public StandardLocalSocks5Handler()
        {

        }
        ~StandardLocalSocks5Handler()
        {
            Cleanup();
        }

        public async Task HandleTcp(IClient client, CancellationToken cancellationToken)
        {
            if (null == client) { return; }
            //negotiate
            if (!await Handshake(client, cancellationToken)) { return; }
            using (SmartBuffer request = SmartBuffer.Rent(300), response = SmartBuffer.Rent(300))
            {
                request.SignificantLength = await client.ReadAsync(request.Memory, cancellationToken);
                if (5 < request.SignificantLength)
                {
                    #region 
                    /*
                    +----+-----+-------+------+----------+----------+
                    |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
                    +----+-----+-------+------+----------+----------+
                    | 1  |  1  | X'00' |  1   | Variable |    2     |
                    +----+-----+-------+------+----------+----------+

                     Where:

                          o  VER    protocol version: X'05'
                          o  CMD
                             o  CONNECT X'01'
                             o  BIND X'02'
                             o  UDP ASSOCIATE X'03'
                          o  RSV    RESERVED
                          o  ATYP   address type of following address
                             o  IP V4 address: X'01'
                             o  DOMAINNAME: X'03'
                             o  IP V6 address: X'04'
                          o  DST.ADDR       desired destination address
                          o  DST.PORT desired destination port in network octet
                             order


                    +----+-----+-------+------+----------+----------+
                    |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
                    +----+-----+-------+------+----------+----------+
                    | 1  |  1  | X'00' |  1   | Variable |    2     |
                    +----+-----+-------+------+----------+----------+

                     Where:

                          o  VER    protocol version: X'05'
                          o  REP    Reply field:
                             o  X'00' succeeded
                             o  X'01' general SOCKS server failure
                             o  X'02' connection not allowed by ruleset
                             o  X'03' Network unreachable
                             o  X'04' Host unreachable
                             o  X'05' Connection refused
                             o  X'06' TTL expired
                             o  X'07' Command not supported
                             o  X'08' Address type not supported
                             o  X'09' to X'FF' unassigned
                          o  RSV    RESERVED
                          o  ATYP   address type of following address
                             o  IP V4 address: X'01'
                             o  DOMAINNAME: X'03'
                             o  IP V6 address: X'04'
                          o  BND.ADDR       server bound address
                          o  BND.PORT       server bound port in network octet order
                     */

                    #endregion
                    var cmd = request.Memory.Span[1];
                    switch (cmd)
                    {
                        case 0x1://connect
                            break;
                        case 0x2://bind
                            {
                                request.Memory.Slice(0, request.SignificantLength).CopyTo(response.Memory);
                                response.Memory.Span[1] = 0x7;
                                await client.WriteAsync(response.Memory.Slice(0, request.SignificantLength), cancellationToken);
                                client.Close();
                            }
                            break;
                        case 0x3://udp assoc
                            //TODO
                            break;
                        default:
                            break;
                    }



                }


            }




            //connect, pipe


            client.Closing += Client_Closing;

            //var relayCllient= await TcpClient1.ConnectAsync()
            //wait for response
            //Cipher
            //Filter
            //

            DefaultPipe pip = null;// new DefaultPipe(tcpClient)

            pip.OnBroken += Pip_OnBroken;
            //pip.ApplyFilter()
            pip.Pipe();

        }



        public async Task HandleUdp(IClient udpClient, CancellationToken cancellationToken = default)
        {
            if (null != udpClient)
            {
                //authentication
                //pipe
            }
        }

        private void Client_Closing(object sender, ClientEventArgs e)
        {
            e.Client.Closing -= this.Client_Closing;
            //do something
            //disassoc udp
        }


        private void Pip_OnBroken(object sender, PipeEventArgs e)
        {
            e.Pipe.OnBroken -= this.Pip_OnBroken;
            this._pipes.Remove(e.Pipe as DefaultPipe);
        }


        static byte[] _handshakeAccept = { 0x5, 0x0 };
        static byte[] _handshakeReject = { 0x5, 0xFF };
        async Task<bool> Handshake(IClient client, CancellationToken cancellationToken)
        {
            using (var request = SmartBuffer.Rent(300))
            {
                if (2 < await client.ReadAsync(request.Memory, cancellationToken))
                {
                    if (request.Memory.Span[0] != 0x5)//accept socks5 only.
                    {
                        await client.WriteAsync(_handshakeReject, cancellationToken);
                        client.Close();
                        return false;
                    }
                    else
                    {
                        return 2 == await client.WriteAsync(_handshakeAccept, cancellationToken);
                    }
                }
                client.Close();
                return false;
            }

        }

        void Cleanup()
        {

        }
        public void Dispose()
        {
            Cleanup();
        }
    }
}
