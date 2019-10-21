using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO;
using System.Timers;
using System.Diagnostics;
using System.Globalization;

namespace TMC
{
    class Udp
    {
        private static System.Timers.Timer TimerPi;
        private static System.Timers.Timer TimerPiStop;
        public static Socket receiver_server;
        private int _buffer;

        // Arrays for ping
        private int[] start_point = new int[10];
        private int[] stop_point = new int[10];

        // Send ping to all players
        public void SendPing()

        {
            string time_now = "";
            Set printData = new Set();
            int num;

            // Share server and client for ping
            if (Data.HostOrClient)
            {
                num = 1;
            }
            else
            {
                num = 0;
            }

            // Sorting through all the participants
            for (int i = num; i < Data.GamersList.gamer.Count; i++)
            {
                // Send a package to everyone except myself
                if (Data.MyNamber != i)
                {
                    try
                    {
                        // Message to send
                        byte[] data = new byte[256];

                        string message = "s" + Data.MyNamber.ToString() + "s";
                        data = Encoding.Unicode.GetBytes(message);

                        if (Data.GamersList.gamer[i].Udp != null)
                        {
                            try
                            {
                                receiver_server.SendTo(data, Data.GamersList.gamer[i].Udp);

                                // Get the first ping point
                                time_now = DateTime.Now.ToString("ss.fff");
                                time_now = time_now.Replace(".", "");
                                start_point[i] = 0;
                                Int32.TryParse(time_now, out start_point[i]);

                            }
                            catch { }

                        }

                        Array.Clear(data, 0, data.Length);
                    }
                    catch { }

                }

            }

        }

        // Start ping
        public void PingAll()
        {

            // Do not wait a second, but immediately send a ping
            SendPing();
            int per = 2;
            per = per * 1000;

            // Set the response interval
            TimerPi = new System.Timers.Timer(per);
            TimerPi.Elapsed += OnTimedEvent;
            TimerPi.AutoReset = true;
            TimerPi.Enabled = true;

            // Second timer monitors the first every half second
            TimerPiStop = new System.Timers.Timer(200);
            TimerPiStop.Elapsed += OnTimedEventStop;
            TimerPiStop.AutoReset = true;
            TimerPiStop.Enabled = true;

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            SendPing();
        }

        private void OnTimedEventStop(Object source, ElapsedEventArgs e)
        {
            if (Data.UdpWorkSend == false)
            {
                TimerPiStop.Stop();
                TimerPiStop.Dispose();
                TimerPiStop.Enabled = false;
                TimerPi.Stop();
                TimerPi.Dispose();
                TimerPi.Enabled = false;

            }
        }


        // Send ping to the player number in the list
        public void SendUdp(int number_list)

        {
            string time_now = "";


            // Create a UdpClient to send messages
            // Send a package to everyone except myself
            if (Data.MyNamber != number_list)
            {

                try

                {

                    // Message to send
                    string message = "s<pi>0->" + number_list + "<po>";

                    byte[] data = Encoding.Unicode.GetBytes(message);

                    // Sending
                    receiver_server.SendTo(data, Data.GamersList.gamer[number_list].Udp);

                    // Get the first ping point
                    time_now = DateTime.Now.ToString("s fff");
                    start_point[number_list] = 0;
                    Int32.TryParse(DateTime.Now.ToString("s fff"), out start_point[number_list]);

                }
                catch (Exception ex) { }

            }

        }


        // Listen and wait for messages
        public void ReceiveMessage()

        {
            receiver_server = null;

            Set printData = new Set();
            int buffer_size = 256;
            bool firstMessLd = true;
            string time_now = "";
            int real_ping;
            IPEndPoint LdIpPort = null;
            string message = "";

            Get get_param = new Get();

            // UDP port FROM FILE for listening from all IPs
            int portReciever = 0;
            if (Data.HostOrClient)
            {

                Int32.TryParse(Data.SettingCh.St.UdpPort, out portReciever);
            }
            else
            {
                Random randomUdpPort = new Random();
                portReciever = randomUdpPort.Next(49152, 65535);

            }

            Data.UdpWorkRec = true;

            int num;
            try
            {
                receiver_server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint localIP = new IPEndPoint(IPAddress.Any, portReciever);
                receiver_server.Bind(localIP);

            }
            catch (Exception ex)
            {
                MainWin.Message("Udp port error", "Failed to determine UDP port, specify it in the settings yourself");
            }



            string[] parts;
            string[] partr;

            // Buffer for received data
            byte[] data = new byte[buffer_size];
            byte[] data_ = new byte[6];

            // Client for mailing to ld (for mailing to other players)
            UdpClient sendToLd = new UdpClient();

            // The address from which the data came
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

            if (Data.HostOrClient)
            {
                num = 1;
            }
            else
            {
                num = 0;
            }

            try
            {
                int bytes;

                // UDP listening cycle starts immediately and works while there is a room
                // first listens to ping, then receives data from clients

                while (Data.UdpWorkRec == true)

                {
                    try
                    {
                        // Get data
                        _buffer = receiver_server.ReceiveFrom(data, buffer_size, SocketFlags.None, ref remoteIp); //  
                    }
                    catch (Exception ex) { }

                    if (data[0] == 's')
                    {

                        // Converted from bytes to string to parse
                        message = Encoding.Unicode.GetString(data, 0, 6);
                        parts = message.Split('s');

                        int number = 0;
                        Int32.TryParse(parts[1], out number);

                        Array.Clear(data, 0, buffer_size);

                        string message2 = "r" + Data.MyNamber + "r";


                        data_ = Encoding.Unicode.GetBytes(message2);
                        try
                        {
                            {
                                receiver_server.SendTo(data_, remoteIp);
                            }
                        }
                        catch (Exception ex) { }

                        // When ping arrives, the host always updates its data
                        if (Data.HostOrClient)
                        {
                            if (Data.LastNumber != 0)
                            {
                                Data.GamersList.gamer[number].Udp = remoteIp as IPEndPoint;

                                // If there is ping with everyone
                                if (get_param.GetReadyPing())
                                {

                                    Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
                                    {

                                        Data.GamersList.Type = 7;
                                        Host.SendAllThread(Data.GamersList);

                                    }));

                                    // After the exchange, discard the last player before another
                                    Data.LastNumber = 0;
                                }
                            }

                        }

                    }
                    else if (data[0] == 'r')
                    {
                        // Get the first ping point
                        time_now = DateTime.Now.ToString("ss.fff");

                        // Converted from bytes to string to parse
                        message = Encoding.Unicode.GetString(data, 0, _buffer);
                        partr = message.Split('r');

                        int number = 0;
                        Int32.TryParse(partr[1], out number);

                        time_now = time_now.Replace(".", "");
                        stop_point[number] = 0;
                        Int32.TryParse(time_now, out stop_point[number]);

                        real_ping = stop_point[number] - start_point[number];
                        try
                        {
                            if (Data.GamersList.gamer[number] != null)
                            {
                                if (real_ping >= 1000)
                                {
                                    double _realPing = real_ping / 1000;
                                    Data.GamersList.gamer[number].GPing = _realPing.ToString("N2");
                                }
                                else if (real_ping >= 10000)
                                {
                                    double _realPing = real_ping / 1000;
                                    Data.GamersList.gamer[number].GPing = _realPing.ToString("N1");
                                }
                                else if (real_ping >= 0)
                                {
                                    Data.GamersList.gamer[number].GPing = real_ping.ToString();

                                }
                            }


                            // If the host then I check for ping always
                            if (Data.HostOrClient)
                            {
                                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                            {
                                Data.LobbyLink.CheckPing();
                            }));
                            }

                        }
                        catch { }

                        Array.Clear(data, 0, buffer_size);

                    }
                    else if (data[0] == 'u')
                    {
                        Set messag = new Set();
                        messag.SetText("UDP port is open");
                    }

                    else if (data[0] < 3)
                    {

                        data[0] += 5;

                        // If the first message, then determine the address
                        if (firstMessLd == true)
                        {
                            // We receive data about LD and we will send them from players
                            LdIpPort = remoteIp as IPEndPoint;
                            firstMessLd = false;
                        }

                        // Mailing to all players except yourself
                        for (int i = num; i < Data.GamersList.gamer.Count; i++)
                        {

                            // Send a package to everyone except myself
                            if (Data.MyNamber != i)
                            {
                                try
                                {
                                    receiver_server.SendTo(data, Data.GamersList.gamer[i].Udp);
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                        }

                        // Clean the buffer
                        Array.Clear(data, 0, buffer_size);
                    }

                    else

                    {

                        data[0] -= 5;

                        if (LdIpPort != null)
                        {
                            sendToLd.Send(data, data.Length, LdIpPort);
                        }

                        // Clean the buffer
                        Array.Clear(data, 0, buffer_size);
                    }
                }

                // Exit the loop and close the listening socket
                if (receiver_server != null)
                    receiver_server.Close();

                // Close the send socket on ld
                if (sendToLd != null)
                    sendToLd.Close();

            }
            catch (Exception ex) { }
        }

        // Start listening to players and ld in a new stream
        public void StartUDPConection()

        {
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }

        // Start pinging in a new thread
        public void StartPing()

        {
            Thread sendThread = new Thread(new ThreadStart(PingAll));
            sendThread.Start();
        }

        public void StartUDPModul()
        {
            Data.UdpWorkSend = true;
            StartUDPConection();
            StartPing();
        }
        public void StartUDPModul0neSec()
        {
            Thread.Sleep(500);
            StartUDPModul();
        }
        public void StartUDPModul0neSecThread()

        {
            Thread sendThread = new Thread(new ThreadStart(StartUDPModul0neSec));
            sendThread.Start();
        }

    }
}
