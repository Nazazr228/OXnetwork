using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerTCP
{
    public class ServerSend
    {
        private static void SendData(int clientId, Packet packet)
        {
            packet.WriteLength();
            Server.Client[clientId].Tcp.SendData(packet);
        }

        private static void SendDataToAll(Packet packet)
        {
            packet.WriteLength();
            foreach (var i in Server.Client)
            {
                i.Value.Tcp.SendData(packet);
            }
        }
        public static void Welcome(int clientId, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(clientId);
                packet.Write(message);
                SendData(clientId, packet);
            }
        }
    }
}
