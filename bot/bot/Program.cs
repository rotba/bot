using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace bot
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient for_cc_port = new UdpClient(0);
            int cc_port = ((IPEndPoint)for_cc_port.Client.LocalEndPoint).Port;
            for_cc_port.Close();
            Console.Write("need to initial this on cc as listining port" + cc_port);
            Thread[] threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                BotServer bot = new BotServer(cc_port);
                threads[i] = new Thread(() => bot.start());
                threads[i].Start();
            }
            bool still_running = true;
            while (still_running)
            {
                still_running = false;
                foreach(Thread t in threads)
                {
                    if (t !=null && t.IsAlive)
                        still_running = true;
                }
            }
        }
    }
}
