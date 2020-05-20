/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Infrastructure.Sockets
{
    /// <summary>
    /// Represent a UDP client.
    /// </summary>
    public class UdpClient1 : Client
    {
        public UdpClient1(Socket socket, ILogger logger = null)
             : base(socket, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="logger"></param>
        /// <returns>null if error.</returns>
        public static async Task<UdpClient1> ConnectAsync(EndPoint remoteEndPoint, ILogger logger = null)
        {
            Throw.IfNull(() => remoteEndPoint);
            try
            {
                /*
                 Connect() method:
                 If you are using a connection-oriented protocol such as TCP, 
                 the Connect method synchronously establishes a network connection between LocalEndPoint and the specified remote host. 
                 If you are using a connectionless protocol, Connect establishes a default remote host. 
                 After you call Connect you can send data to the remote device with the Send method, 
                 or receive data from the remote device with the Receive method.

                 If you are using a connectionless protocol such as UDP, you do not have to call Connect 
                 before sending and receiving data. You can use SendTo and ReceiveFrom to synchronously communicate with a remote host. 
                 If you do call Connect any datagrams that arrive from an address other than the specified default will be discarded. 
                 If you want to set your default remote host to a broadcast address, you must first call the SetSocketOption method 
                 and set the socket option to SocketOptionName.Broadcast, or Connect will throw a SocketException. 

                If you are using a connection-oriented protocol and did not call Bind before calling Connect, 
                the underlying service provider will assign the local network address and port number. 
                If you are using a connectionless protocol, the service provider will not assign a local network address and port number 
                until you complete a send or receive operation. If you want to change the default remote host, 
                call Connect again with the desired endpoint.

                If the socket has been previously disconnected, then you cannot use this method to restore the connection. 
                Use one of the asynchronous BeginConnect methods to reconnect. This is a limitation of the underlying provider.
                 
                 
                Bind() method:
                Use the Bind method if you need to use a specific local endpoint. You must call Bind before you can call the Listen method. 
                You do not need to call Bind before using the Connect method unless you need to use a specific local endpoint. 
                You can use the Bind method on both connectionless and connection-oriented protocols.

                Before calling Bind, you must first create the local IPEndPoint from which you intend to communicate data. 
                If you do not care which local address is assigned, you can create an IPEndPoint using IPAddress.Any as the address parameter, 
                and the underlying service provider will assign the most appropriate network address. 
                This might help simplify your application if you have multiple network interfaces. 
                If you do not care which local port is used, you can create an IPEndPoint using 0 for the port number. 
                In this case, the service provider will assign an available port number between 1024 and 5000.
                 
                If you use the above approach, you can discover what local network address and port number has been assigned by calling the LocalEndPoint. 
                If you are using a connection-oriented protocol, LocalEndPoint will not return the locally assigned network address
                until after you have made a call to the Connect or EndConnect method. 
                If you are using a connectionless protocol, you will not have access to this information until you have completed a send or receive.

                If a UDP socket wants to receive interface information on received packets, the SetSocketOption method should be explicitly called 
                with the socket option set to PacketInformation immediately after calling the Bind method.

                If you intend to receive multicast datagrams, you must call the Bind method with a multicast port number.

                You must call the Bind method if you intend to receive connectionless datagrams using the ReceiveFrom method.
                
                */

                //UdpClient udpClient = null;                
                Socket sock = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                await sock.ConnectAsync(remoteEndPoint);
                sock.DontFragment = true;
                sock.ReceiveTimeout = Defaults.ReceiveTimeout;
                sock.SendTimeout = Defaults.SendTimeout;

                return new UdpClient1(sock, logger);
            }
            catch (SocketException se)
            {
                logger?.LogError($"UdpClient1 ConnectAsync error {se.SocketErrorCode}, {se.Message}.");
                return null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"UdpClient1 ConnectAsync error.");
                return null;
            }
        }


    }
}
