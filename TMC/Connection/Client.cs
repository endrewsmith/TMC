using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Globalization;
using System.Collections.ObjectModel;

using WpfAnimatedGif;


namespace TMC
{
    class Client
    {
        //public static ImageBrush myBrush;
        public delegate void UpdateLogCallback(GamerList gamerList);
        StreamReader srReceiver;
        private static TcpClient _tcpServer;
        private static IPAddress _ipHost;
        private static Thread _thrMessaging;
        private static NetworkStream _tcpStream;

        // Switch for receiving messages
        public bool Connected;

        // Random TCP port function
        public bool TryConectToRandomPort()

        {
            Set printData = new Set();
            try
            {
                if (_tcpServer != null)
                {
                    _tcpServer.Close();
                    _tcpServer = null;
                }

                Random randomTcpPort = new Random();
                int myTcpPort;
                myTcpPort = randomTcpPort.Next(49152, 65535);
                IPEndPoint localIP = new IPEndPoint(IPAddress.Any, myTcpPort);
                _tcpServer = new TcpClient(localIP);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Random TCP port function for iteration times
        public bool TryConectToRandomPortIteration(int iteration)

        {
            for (int i = 0; i < iteration; i++)
            {
                if (TryConectToRandomPort())
                {
                    return true;
                }
            }
            return false;
        }


        // Initialization
        public void InitializeConnection(string ip, int port)

        {
            _tcpServer = null;
            Set printData = new Set();

            try
            {
                TryConectToRandomPortIteration(10);
            }

            // If it didn’t succeed in contacting the specified port,
            // the program tries random ports (10 iterations)
            catch (Exception ex)
            {

                if (!TryConectToRandomPortIteration(5))
                {

                    // If it didn’t turn out to find the port, we tell the player about it and exit
                    MainWin.Message("TCP error", "Did not manage to pick up tcp port for a connection");

                    if (_tcpServer != null)
                        _tcpServer.Close();

                    return;
                }

            }

            try
            {

                Connected = true;

                 _ipHost = IPAddress.Parse(ip);
                IAsyncResult result = _tcpServer.BeginConnect( _ipHost, port, null, null);

                // Set timeout for connection
                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (!_tcpServer.Connected)
                {
                    CloseConnection("Connection error", "You cannot join this lobby. Some problem with the TCP port");
                    return;
                }

            }

            // If it didn’t succeed in connecting, show a message
            catch (Exception ex)
            {
                CloseConnection("Connection error", "You cannot join this lobby. Some problem with the TCP");
                return;
            }

            _tcpStream = _tcpServer.GetStream();

            // Create an object for serialization:
            Gamers Gamer = new Gamers();
            if (!Data.JoinToServer)
            {
                Gamer.Key = "yes";
            }
            Gamer.Type = 0;
            Gamer.Name = Data.SettingCh.St.Name;
            Gamer.Status = 0;
            Gamer.Car = Data.SettingCh.St.Car;
            Gamer.Team = Data.SettingCh.St.Team;
            Gamer.Message = ip;

            if (Data.JoinToServer)
            {
                if (Data.webList[Data.LobbyNumber].PasswordInp == null)
                {
                    Gamer.Password = "";
                }
                else
                {
                    Gamer.Password = Data.webList[Data.LobbyNumber].PasswordInp;
                }
            }

            // Sending to client
            _tcpStream.Write(Json.WriteClient(Gamer), 0, Json.WriteClient(Gamer).Length);
            _tcpStream.Flush();


            // Open thread reciaving messages
            _thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
            _thrMessaging.Start();

        }

        private void OnTcpClientConnected(IAsyncResult asyncResult)
        {
            try
            {
                using (var tcpClient = (TcpClient)asyncResult.AsyncState)
                {
                    tcpClient.EndConnect(asyncResult);
                    var ipAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                    var stream = tcpClient.GetStream();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        // For sending messages
        public void SendMessages(int typeMassage, Gamers gamerSender)
        {
            try
            {
                gamerSender.Type = typeMassage;
                gamerSender.Number = Data.MyNamber;
                _tcpStream.Write(Json.WriteClient(gamerSender), 0, Json.WriteClient(gamerSender).Length);
                _tcpStream.Flush();

            }
            catch

            { }
        }

        // For each message its own stream
        public void SendMessagesThread(int typeMassage, Gamers gamerSender)

        {
            Client send = new Client();
            Thread sendThread = new Thread(delegate () { send.SendMessages(typeMassage, gamerSender); });
            sendThread.Start();
        }

        // For receiving mssages
        private void ReceiveMessages()

        {
            int bufferSize = 8000;
            Set printData = new Set();
            Set setGamer = new Set();

            // Create a buffer for receiving 
            byte[] bytes = new byte[bufferSize];

            _tcpStream = _tcpServer.GetStream();

            try
            {
                int bytesRead = _tcpStream.Read(bytes, 0, bytes.Length);
            }
            catch { }

            GamerList gamerTempList = new GamerList();
            gamerTempList = Json.ReadHost(bytes);

            if (gamerTempList == null)
            {
                MainWin.Message("Connection error", "Connection error");
                // Exit
                return;
            }

            switch (gamerTempList.Type)
            {
                // Error in nikename
                case -4:
                    {
                        CloseConnection("Connection error", "Invalid characters in name");
                        return;
                    }
                // Closing room
                case -3:
                    {
                        CloseConnection("Connection error", "Lobby closed");
                        return;
                    }
                // Wrong password    
                case -2:
                    {
                        CloseConnection("Connection error", "Wrong password");
                        return;
                    }
                // Name is already used
                case -1:
                    {
                        CloseConnection("Connection error", "Your name is already used");
                        return;
                    }

                // If there are no errors and the request is accepted
                default:
                    {
                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            // Create lobby window
                            Data.MainWin.CreateLobby();

                        }));

                        // Accept the list of all players
                        Data.GamersList = gamerTempList;

                        printData.SetMassageInfo("You are logged in as " + Data.SettingCh.St.Name + "\r\n");

                        // Display the list in the table
                        for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                        {
                            setGamer.AddToGrid(i);
                        }

                        // Get your number
                        Get getMyNumber = new Get();
                        getMyNumber.GetMyNamber();

                        Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Get getMap = new Get();

                            // Set map
                            Data.LobbyLink.ShowMap(Data.GamersList.Map);

                            // Set score
                            Data.LobbyLink.ScoreChange(Data.GamersList.Score);

                            // Set header
                            if (Data.JoinToServer)
                            {
                                string name;
                                if (Data.webList[Data.LobbyNumber].Name.Length > 16)
                                {
                                    name = Data.webList[Data.LobbyNumber].Name.Substring(0, 16) + "...";

                                }
                                else
                                {
                                    name = Data.webList[Data.LobbyNumber].Name;
                                }

                                Data.LobbyLink.Title = name + " [ " + Data.GamersList.ModOrig + " | " + Data.GamersList.LobbyType + " ]";
                            }
                            else
                            {
                                Data.LobbyLink.Title = Data.GamersList.gamer[0].Name + "'s Lobby [ " + Data.GamersList.ModOrig + " | " + Data.GamersList.LobbyType + " ]";
                            }

                        }));

                        // Turn on UDP module
                        Udp UdpOn = new Udp();
                        UdpOn.StartUDPModul();

                        srReceiver = new System.IO.StreamReader(_tcpStream);

                        // While Connected == true accept messages
                        while (Connected)
                        {
                            try
                            {
                                int byteRead;
                                byte[] bytesHost = new byte[bufferSize];

                                byteRead = _tcpStream.Read(bytesHost, 0, bufferSize);

                                if (byteRead != 0)
                                {
                                    GamerList gamerList = new GamerList();
                                    gamerList = Json.ReadHost(bytesHost);

                                    Data.LobbyLink.Dispatcher.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { gamerList });
                                }

                                else
                                {
                                    CloseConnection("Connection error", "The lobby is closed by host");
                                    return;
                                }

                            }

                            catch { return; }

                        }

                        return;
                    }
            }

        }

        // Processing messages from players
        private void UpdateLog(GamerList gamerList)
        {
            Set printData = new Set();
            Set addToDataGrid = new Set();

            switch (gamerList.Type)
            {
                // Update list gamers
                case 0:
                    {

                        Data.gamerList.Clear();
                        Data.GamersList = gamerList;

                        // Get your number
                        Get getMyNumber = new Get();
                        getMyNumber.GetMyNamber();

                        // Host sending 
                        for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                        {
                            addToDataGrid.AddToGrid(i);
                        }

                        // Exit the method
                        return;
                    }

                // Changing map    
                case 1:
                    {
                        Data.SettingCh.St.Map = gamerList.Map;
                        Data.LobbyLink.ShowMap(gamerList.Map);

                        // Exit the method
                        return;
                    }

                // Changing car 
                case 2:
                    {

                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.GamersList.gamer[gamerList.Number].GCar = gamerList.Map;
                        }));

                        // Exit the method
                        return;
                    }

                // Changing team 
                case 11:
                    {

                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.GamersList.gamer[gamerList.Number].GTeam = gamerList.Map;
                        }));

                        // Exit the method
                        return;
                    }

                // Changing gamers status
                case 3:
                    {

                        Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.GamersList.gamer[gamerList.Number].GStatus = gamerList.Map;

                            if (Data.GamersList.gamer[Data.MyNamber].Status == 0)
                            {
                                Data.LobbyLink.UnLock();
                            }
                            else
                            {
                                Data.LobbyLink.Lock();
                            }

                            if (Data.GamersList.gamer[0].Status != 0 && Data.GamersList.gamer[Data.MyNamber].Status != 0)
                            {
                                Data.BtnReady.IsEnabled = false;
                                Data.LineBtnReady.IsEnabled = false;
                            }
                            else
                            {
                                Data.BtnReady.IsEnabled = true;
                                Data.LineBtnReady.IsEnabled = true;
                            }
                        }));

                        // Exit the method
                        return;
                    }

                // Set message from gamer
                case 4:
                    {

                        Set setMassage = new Set();
                        setMassage.SetMassage(gamerList.Number, gamerList.Message);

                        // Exit the method
                        return;
                    }

                // Set message abot already taken car
                case 5:
                    {

                        Set MassageAboutCar = new Set();
                        MassageAboutCar.SetMassageCar(gamerList.Number);

                        // Exit the method
                        return;
                    }

                // Changing score
                case 6:
                    {
                        Data.SettingCh.St.Score = gamerList.Score;

                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.LobbyLink.ScoreChange(gamerList.Score);
                            Sound.PlaySound("score.mp3");

                        }));

                        // Exit the method
                        return;
                    }

                // Changing UDP ports
                case 7:
                    {

                        for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                        {
                            Data.GamersList.gamer[i].Udp = gamerList.gamer[i].Udp;
                        }

                        // Exit the method
                        return;
                    }

                // Start ld.exe
                case 8:
                    {
                        // Everyone has the same data
                        Data.GamersList = gamerList;
                        Ld startLd = new Ld();

                        Sound.PlaySound("go.mp3");

                        startLd.StartLDThread();

                        return;
                    }

                // Add or leave lobby
                case 9:
                    {
                        // Everyone has the same data
                        Set massageAddLeave = new Set();

                        if (gamerList.Message == "add")
                            massageAddLeave.SetMassageAddLeave(true, gamerList.Number);
                        else
                            massageAddLeave.SetMassageAddLeave(false, gamerList.Number);

                        return;
                    }

                // Close connection
                case 10:
                    {
                        CloseConnection("Connection error", "The host terminated your connection");

                        // Exit the method
                        return;
                    }

                case 12:
                    {
                        CloseConnection("Connection error", "The lobby is closed");

                        // Exit the method
                        return;
                    }

                // Reboot UDP modul
                case 13:
                    {
                        // Turn off UDP
                        Data.UdpWorkSend = false;
                        Data.UdpWorkRec = false;
                        if (Udp.receiver_server != null)
                        {
                            Udp.receiver_server.Close();
                        }

                        // Turn on UDP module
                        Udp UdpOn = new Udp();
                        UdpOn.StartUDPModul0neSecThread();

                        // Exit the method
                        return;
                    }

                default:
                    {
                        return;
                    }
            }

        }

        // Closing connection
        public void CloseConnection(string caption, string text)

        {
            Data.UdpWorkSend = false;

            try
            {
                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {
                    // Clear list
                    Data.gamerList.Clear();
                    Data.LobbyLink.Title = caption + ": " + text;

                }));

                try
                {

                    Connected = false;

                    if (_tcpStream != null)
                    {
                        _tcpStream.Flush();
                        _tcpStream.Close();
                    }

                    if (_tcpServer != null)
                    {
                        _tcpServer.Close();
                    }

                    if (srReceiver != null)
                        srReceiver.Close();

                }
                catch { }

                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {

                    Data.LobbyLink.Close();
                    Data.MainWin.Focus();

                }));

                // Show message about reason of closing
                if (text != "The lobby is closed by host")
                    MainWin.Message(caption, text);

            }
            catch { }


            Data.UdpWorkRec = false;
            if (Udp.receiver_server != null)
            {
                Udp.receiver_server.Close();
            }
        }


    }
}
