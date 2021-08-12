using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ClientTCP
{
    public class Client
    {
        public Client()
        {
            Tcp = new TCP();
        }

        public static Client Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Client();

                return _instance;
            }
        }
        public int Port = 1;
        private static Client _instance;
        public static int DataBufferSize = 16 * 16 * 16;
        public int MyId;
        public TCP Tcp;
        public string IP = "127.0.0.1";
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> _packetHandler = new Dictionary<int, PacketHandler>()
        {
            [(int)ServerPackets.welcome] = ClientHandel.Welcome
        };

        public class TCP
        {
            public TcpClient Socket;
            private NetworkStream _networkStream;
            private byte[] _recieveBuffer;
            private Packet _recievedData;

            public void Connect()
            {
                Socket = new TcpClient()
                {
                    ReceiveBufferSize = DataBufferSize,
                    SendBufferSize = DataBufferSize
                };
                _recieveBuffer = new byte[DataBufferSize];
                Socket.BeginConnect(Instance.IP, Instance.Port, ConnectCallback, Socket);
            }

            private void ConnectCallback(IAsyncResult result)
            {
                Socket.EndConnect(result);

                if (!Socket.Connected)
                {
                    Console.WriteLine("not connect to server");
                    return;
                }
                _networkStream = Socket.GetStream();
                _recievedData = new Packet();
                _networkStream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
            }

            private void RecieveCallback(IAsyncResult result)
            {

                try
                {
                    int byteLenght = _networkStream.EndRead(result);
                    if (byteLenght <= 0)
                    {
                        return;
                    }
                    byte[] data = new byte[byteLenght];
                    Array.Copy(_recieveBuffer, data, byteLenght);
                    _recievedData.Reset(HandleData(data));
                    _networkStream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
                }

                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLends = 0;
                _recievedData.SetBytes(data);

                if (_recievedData.UnreadLength() >= 4)
                {
                    packetLends = _recievedData.ReadInt();
                    if (packetLends <= 0)
                    {
                        return true;
                    }
                }

                while (packetLends > 0 && packetLends <= _recievedData.UnreadLength())
                {
                    byte[] packetBytes = _recievedData.ReadBytes(packetLends);
                    using(Packet packet= new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        _packetHandler[packetId](packet);
                    }
                    packetLends = 0; 

                    if (_recievedData.UnreadLength() >= 4)
                    {
                        packetLends = _recievedData.ReadInt();

                        if (packetLends <= 0)
                        {
                            return true;
                        }
                    }

                }

                if (packetLends<=1)
                {
                    return true;
                }
                return false;
            }
        }

        public void ConnectToServer()
        {
            Tcp.Connect();
        }
    }
}
