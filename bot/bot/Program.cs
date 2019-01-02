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
            int num_of_bots = 10;
            int cc_port = 31337;
            Thread[] threads = new Thread[num_of_bots];
            for (int i = 0; i < threads.Length; i++)
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
