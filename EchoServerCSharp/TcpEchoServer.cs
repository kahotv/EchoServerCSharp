using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EchoServerCSharp
{
    internal class TcpEchoServer : IEchoServer
    {
        public async Task Start(ushort port)
        {
            var task4 = Start(AddressFamily.InterNetwork, port);
            var task6 = Start(AddressFamily.InterNetworkV6, port);

            await Task.WhenAny(task4, task6);
        }


        async Task Start(AddressFamily family, ushort port)
        {
            try
            {
                EndPoint epListen = new IPEndPoint(family == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, port);
                Socket sock = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
                sock.Bind(epListen);
                sock.Listen();

                Console.WriteLine("[TcpEchoServer] listen {0}", epListen.ToString());

                while (true)
                {
                    Socket cli = await sock.AcceptAsync();

                    _ = StartSession(cli);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[TcpEchoServer] error {0}", e);
            }

        }

        async Task StartSession(Socket sock)
        {
            Console.WriteLine("[TCP][{0} -> {1}] accepted.", sock.RemoteEndPoint, sock.LocalEndPoint);
            try
            {
                byte[] buf = new byte[0x1000];
                while (true)
                {
                    int n = await sock.ReceiveAsync(new Memory<byte>(buf));

                    Console.WriteLine("[TCP][{0} -> {1}] read len: {2}\n{3}",
                        sock.RemoteEndPoint, sock.LocalEndPoint,
                        n, HexDump.HexDump.Format(buf.Take(n).ToArray()));

                    await sock.SendAsync(new ReadOnlyMemory<byte>(buf, 0, n));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[TCP][{0} -> {1}] closed.", sock.RemoteEndPoint, sock.LocalEndPoint);
                return;
            }
        }
    }
}
