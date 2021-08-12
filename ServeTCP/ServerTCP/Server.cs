using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCP
{
    public class Server
    {
        public static int MaxPlayers;
        public static int Port;
        private static TcpListener _tcpListener;
        public static Dictionary<int, Client> Client = new Dictionary<int, Client>();
        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;
            Console.WriteLine("server start");
            InitializeServer();
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TcpConnectionCallback, null);
        }

        private static void TcpConnectionCallback(IAsyncResult result)
        {
            TcpClient tcpClient = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TcpConnectionCallback, null);
            Console.WriteLine($"new client {tcpClient.Client.RemoteEndPoint} ");
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (Client[i].Tcp.Socket==null )
                {
                    Client[i].Tcp.Connect(tcpClient);
                    return;
                }
            }
            Console.WriteLine($"server is full {tcpClient.Client.RemoteEndPoint} can't connect");
        }

        private static void InitializeServer()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                Client.Add(i, new Client(i));
            }
        }
    }
}
