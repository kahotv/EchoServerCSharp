using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServerCSharp
{
    internal class UdpEchoServer : IEchoServer
    {
        async Task IEchoServer.Start(ushort port)
        {
            var task4 = Start(AddressFamily.InterNetwork, port);
            var task6 = Start(AddressFamily.InterNetworkV6, port);

            await Task.WhenAny(task4, task6);
        }

        async Task Start(AddressFamily family,ushort port)
        {
            try
            {
                IPAddress addr = family == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any;

                Socket sock = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(new IPEndPoint(addr, port));

                Console.WriteLine("[UdpEchoServer] bind {0}", sock.LocalEndPoint);

                byte[] data = new byte[0x10000];
                IPEndPoint epRemote = new IPEndPoint(addr, 0);
                while (true)
                {
                    var r = await sock.ReceiveFromAsync(data, epRemote);
                    Console.WriteLine("from {0}, len {1}\n{2}",
                        r.RemoteEndPoint.ToString(), 
                        r.ReceivedBytes, 
                        HexDump.HexDump.Format(data.Take(r.ReceivedBytes).ToArray()));
                    await sock.SendToAsync(new ReadOnlyMemory<byte>(data, 0, r.ReceivedBytes), r.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[UdpEchoServer] error: {e}");
            }
        }
    }
}
