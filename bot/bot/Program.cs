using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bot
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress newaddress = IPAddress.Parse("192.168.1.1");
            IPEndPoint ipep =new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            Socket server = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            byte[] buffer = Encoding.ASCII.GetBytes("INBI");
            server.Send(buffer, 4, SocketFlags.None);
            while (true) { }
        }
    }
}
