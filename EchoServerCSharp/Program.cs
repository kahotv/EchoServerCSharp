namespace EchoServerCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ushort port = 8000;
            if(args.Length >= 1)
            {
                port = ushort.Parse(args[0]);
            }


            IEchoServer udpEchoServer = new UdpEchoServer();
            IEchoServer tcpEchoServer = new TcpEchoServer();

            var taskUdp = udpEchoServer.Start(port);
            var taskTcp = tcpEchoServer.Start(port);

            Task.WaitAny(taskTcp, taskUdp);

            return;
        }
    }
}
