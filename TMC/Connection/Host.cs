using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Globalization;
using System.Windows.Media;
using System.Windows;





namespace TMC
{
    public class Host
    {
        public static NetworkStream tcpStream;
        public static bool getMyIp;
        public static bool ServRunning;

        // The stream that holds the players joining hearing
        public static Thread thrListener;

        // TCP object that listens for connections
        public static TcpListener tlsClient;

        public static TcpClient tcpClient;
        public static List<TcpClient> tcpClients = new List<TcpClient>();

        // Starts the server and starts receiving clients
        public void StartListening(string port)
        {
            // Before determining its IP, this variable is false
            getMyIp = false;
            Set message = new Set();
            int portInt32 = 0;
            Int32.TryParse(port, out portInt32);

            // Configure sockets over TCP, use IP and open TCP port
            try
            {
                tlsClient = new TcpListener(portInt32);
            }
            catch
            {
                MainWin.Message("Connection error", "Wrong Tcp port");
            }

            // Add the first term equal to zero instead of ourselves
            tcpClients.Add(null);

            // Start to listen
            try
            {
                tlsClient.Start();

                // Create lobby window

                Data.MainWin.CreateLobby();
                Data.MyNamber = 0;

                message.SetMassageInfo("Waiting other players...\r\n");

                // Set a unique session number and will transmit it
                Data.GamersList.IdSession = DateTime.Now.ToString("dd hh:mm:ss").Replace(" ", "").Replace(".", "").Replace(":", "");

                //_____________ Connect the UDP module ___________
                Udp UdpOn = new Udp();
                UdpOn.StartUDPModul();

            }
            catch
            {
                MainWin.Message("Connection error", "Wrong Tcp port [2]");
                return;
            }

            // UDP port FROM FILE for listening from all IPs
            int portUdp = 0;
            Int32.TryParse(Data.SettingCh.St.UdpPort, out portUdp);

            IPEndPoint RemoteEndPoint = null;
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portUdp);

            Data.GamersList.gamer.Add(new Gamers() { Type = 0, Ping = "", Name = Data.SettingCh.St.Name, Number = 0, Status = 0, Car = Data.SettingCh.St.Car, Team = Data.SettingCh.St.Team, Udp = null, Message = null, Password = null, });
            Data.GamersList.Map = Data.SettingCh.St.Map;
            Data.GamersList.Score = Data.SettingCh.St.Score;

            // if created on the server, then enable updates to the server
            if (Data.CreateToServer)
            {
                // Create lobby
                Web createLobby = new Web();
                // This variable is responsible for updating
                Data.FollowToLobby = true;
                // Start creating and updating the lobby in a new thread
                createLobby.CreateLobbyInThread();

            }

            Data.gamerList.Add(Data.GamersList.gamer[0]);

            // Create a new thread for the connection wait loop
            thrListener = new Thread(KeepListening);
            thrListener.Start();


        }

        // Stop accepting customers (close the room from others)
        public void StopListening()
        {
            ServRunning = false;

            if (tlsClient != null)
            {
                tlsClient.Stop();
                tlsClient = null;
            }

        }

        public void CloseC()
        {
            for (int i = 1; i < tcpClients.Count; i++)
            {
                tcpClients[i].Close();
            }
            tcpClients.Clear();

            if (tcpClient != null)
            {
                tcpClient.Close();
            }

            if (thrListener != null)
                thrListener.Abort();

        }

        // Creates a wiretap loop
        private void KeepListening()
        {
            // translate the variable to "true" to establish a connection in a loop
            ServRunning = true;

            // Accepted the connection
            tlsClient.BeginAcceptTcpClient(EndAcceptTcpClient, tlsClient);

        }

        static void EndAcceptTcpClient(IAsyncResult state)
        {

            tlsClient = (TcpListener)state.AsyncState;

            try
            {

                tcpClient = tlsClient.EndAcceptTcpClient(state);
                LingerOption lingerOption = new LingerOption(true, 1);

                tcpClient.LingerState = lingerOption;

                // Created a new variable of the Connection class based on the created connection
                HConnection newConnection = new HConnection(tcpClient);
            }
            catch (InvalidOperationException) { }

            finally
            {


                if (ServRunning == true)
                    tlsClient.BeginAcceptTcpClient(EndAcceptTcpClient, tlsClient);

            }


        }

        // Add the client to the lists
        public static void AddUser(Gamers gamer)

        {
            Set printData = new Set();
            Get closingOrNotLobby = new Get();

            // Number which I will assign to the new player
            int gamerNumber;

            // Add to the list of players and assign a number

            Data.GamersList.gamer.Add(gamer);
            gamerNumber = Data.GamersList.gamer.Count - 1;
            Data.GamersList.gamer[gamerNumber].Number = gamerNumber;

            // The player becomes the last in the lobby
            Data.LastNumber = gamerNumber;

            // Add tables from another stream to the collection
            Data.Inf.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.gamerList.Add(Data.GamersList.gamer[gamerNumber]);

            }));


            // Showing who joined
            printData.SetMassageAddLeave(true, Data.LastNumber);

            // Send a new list to all players
            Data.GamersList.Type = 0;
            SendAllThread(Data.GamersList);

            // Close the Ready button until ping appears
            Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.LobbyLink.CheckPing();
                Data.LobbyLink.FlashWin();
            }));

            // Close the lobby or not
            closingOrNotLobby.GetClosingLobby();

            // Control the start button
            Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.LobbyLink.Go();

            }));

        }

        // Removing a player over TCP (basic removal)
        public static void RemoveUser(TcpClient tcpUser)
        {
            Set print = new Set();
            Set printData = new Set();

            for (int i = 1; i < Data.GamersList.gamer.Count; i++)

                // If the participant is in the tcp list of clients, then delete it by index
                if (tcpClients[i] == tcpUser)
                {

                    //  First show that the participant has left the chat

                    printData.SetMassageAddLeave(false, i);
                    GamerList gamerListAdd = new GamerList();
                    gamerListAdd.Type = 9;
                    gamerListAdd.Number = i;
                    gamerListAdd.Message = "leave";

                    Host.SendAllThread(gamerListAdd);

                    try
                    {
                        // Delete all information from the list
                        Data.GamersList.gamer.RemoveAt(i);

                        // If not the last one in the list has left, then I recount the numbers after it
                        if (i != Data.GamersList.gamer.Count)
                            for (int j = i; j < Data.GamersList.gamer.Count; j++)
                            {
                                Data.GamersList.gamer[j].Number = j;
                            }


                        // Delete from the table collection
                        Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.gamerList.Clear();
                            Set addToDataGrid = new Set();
                            // Host sending 
                            for (int gamer = 0; gamer < Data.GamersList.gamer.Count; gamer++)
                            {
                                addToDataGrid.AddToGrid(gamer);
                            }

                        }));

                    }
                    catch (Exception ex) { }

                    if (tcpUser != null)
                    {
                        tcpUser.Close();
                    }

                    if (tcpClients[i] != null)
                    {
                        tcpClients[i].Close();
                        tcpClients.RemoveAt(i);
                    }

                    Get closingOrNotLobby = new Get();

                    // Open the lobby if the player leaves the lobby himself, if he doesn’t open, the lobby will not open
                    closingOrNotLobby.GetClosingLobby();

                    Data.GamersList.Type = 0;
                    Host.SendAllThread(Data.GamersList);

                    // Control the start button
                    Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                    {
                        Data.LobbyLink.Go();
                    }));
                    Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                    {
                        Data.LobbyLink.CheckPing();
                    }));

                }

        }

        // Removing a player over TCP (basic removal)
        public static void RemoveUserNumber(int index)
        {

            Set printData = new Set();

            // If the participant is in the tcp list of clients, then delete it by index
            if (Host.tcpClients.Count >= (index + 1))
            {

                GamerList gamer = new GamerList();
                gamer.Type = 10;
                SendToNumber(index, gamer);

                // First we’ll show that the participant has left the chat
                printData.SetMassageAddLeave(false, index);
                GamerList gamerListAdd = new GamerList();
                gamerListAdd.Type = 9;
                gamerListAdd.Number = index;
                gamerListAdd.Message = "leave";
                Host.SendAllThread(gamerListAdd);

                try
                {
                    // Delete all information from the list
                    Data.GamersList.gamer.RemoveAt(index);

                    // If not the last one in the list has left, then I recount the numbers after it
                    if (index != Data.GamersList.gamer.Count)
                    {
                        for (int j = index; j < Data.GamersList.gamer.Count; j++)
                        {

                            Data.GamersList.gamer[j].Number = j;
                        }
                    }

                    // Delete from the table collection
                    Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
                        {
                            Data.gamerList.Clear();
                            Set addToDataGrid = new Set();
                            // Host sending 
                            for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                            {
                                addToDataGrid.AddToGrid(i);
                            }

                        }));
                }
                catch (Exception ex) { }

                if (tcpClients[index] != null)
                {
                    tcpClients[index].Close();
                    tcpClients.RemoveAt(index);
                }

                Data.GamersList.Type = 0;
                Host.SendAllThread(Data.GamersList);

                // Control the start button
                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.LobbyLink.Go();
                }));
                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.LobbyLink.CheckPing();
                }));
            }
        }

        public static void SendAll(GamerList gamerList)
        {
            Set printData = new Set();

            for (int i = 1; i < Data.GamersList.gamer.Count; i++)
            {
                try
                {
                    tcpStream = tcpClients[i].GetStream();
                    // Buffer array to verify that I'm sending
                    byte[] send = Json.WriteHost(gamerList);

                    if (send != null)
                    {
                        tcpStream.Write(send, 0, send.Length);
                        tcpStream.Flush();
                    }
                    else
                    {

                        Data.GamersList.Message = null;
                    }

                }
                catch
                {
                    RemoveUserNumber(i);
                }
            }

        }

        public static void SendAllThread(GamerList gamerList)

        {
            Thread sendThread = new Thread(delegate () { SendAll(gamerList); });
            sendThread.Start();
        }


        public static void SendToNumber(int index, GamerList gamerList)
        {
            try
            {
                tcpStream = Host.tcpClients[index].GetStream();

                // Sending
                tcpStream.Write(Json.WriteHost(gamerList), 0, Json.WriteHost(gamerList).Length);
                tcpStream.Flush();

            }
            catch { }
        }
        public static void SendToNumberThread(int index, GamerList gamerList)

        {
            Thread sendThread = new Thread(delegate () { SendToNumber(index, gamerList); });
            sendThread.Start();
        }



    }
}
