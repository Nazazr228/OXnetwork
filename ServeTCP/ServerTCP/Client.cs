using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ServerTCP
{
    public class Client
    {
        public static int DataBufferSize = 16 * 16 * 16;
        public int Id;
        public TCP Tcp;
        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(id);
        }
        public class TCP
        {
            public TcpClient Socket;
            private readonly int _id;
            private NetworkStream _networkStream;
            private byte[] _recieveBuffer;

            public TCP(int ID)
            {
                _id = ID;
            }
            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket!=null)
                    {
                        _networkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"In trying send massage an error has occurred{exc}");
                }
            }

            public void Connect(TcpClient tcpClient)
            {
                Socket = tcpClient;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;
                _networkStream = Socket.GetStream();
                _recieveBuffer = new byte[DataBufferSize];
                _networkStream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
                ServerSend.Welcome(_id, "hello");
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
                    _networkStream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}
