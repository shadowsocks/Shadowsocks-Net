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


    class StandardLocalSocks5Handler : ISocks5Handler
    {
        //accept->add
        //error->remove
        //expire->remove
        // ClientCollection<TcpClient1> _tcpClients = new ClientCollection<TcpClient1>();
        //TcpServer tcpServer

        //tcp lost->remove udp
        //pipe broken -> remove

        List<DefaultPipe> _pipes = new List<DefaultPipe>();



        public async Task HandleTcp(IClient tcpClient)
        {
            if (null != tcpClient)
            {
                //negotiate
                //connect, pipe


                tcpClient.Closing += Client_Closing;

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
        }



        public async Task HandelUdp(IClient udpClient)
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
        }


        private void Pip_OnBroken(object sender, PipeEventArgs e)
        {
            e.Pipe.OnBroken -= this.Pip_OnBroken;
            this._pipes.Remove(e.Pipe as DefaultPipe);
        }
    }
}
