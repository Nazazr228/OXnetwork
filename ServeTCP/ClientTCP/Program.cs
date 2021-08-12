using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ClientTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Client.Instance.ConnectToServer();

            Console.ReadLine();
        }
    }
}
