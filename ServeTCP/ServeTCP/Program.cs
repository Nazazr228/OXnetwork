using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServeTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8080;

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var tcpSocet = new Socket(AddressFaily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocet.Bind(tcpEndPoint);
            tcpSocet.Listen(5);

            while (true)
            {
                var listener = tcpSocet.Accept();
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                Console.WriteLine(data);

                listener.Send(Encoding.UTF8.GetBytes("успех"));

                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
    }
}
