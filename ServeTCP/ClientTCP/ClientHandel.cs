using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ClientTCP
{
    public class ClientHandel
    {
        public static void Welcome(Packet packet)
        {
            int myId = packet.ReadInt();
            string message = packet.ReadString();
            Console.WriteLine(message);
            Client.Instance.MyId = myId;
        }
    }
}
