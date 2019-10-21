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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TMC
{
    public class HConnection
    {
        TcpClient tcpClient;
        NetworkStream tcpStream;
        StreamReader srReceiver;

        // The thread will send messages to the client
        public static Thread thrSender;
        private Gamers _gamerReceiver;

        // Class constructor accepts TCP connection
        public HConnection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;

            // A thread that receives a client and waits for messages
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }

        // Close the connection
        public void CloseConnection()
        {
            tcpStream.Close();
            tcpClient.Close();
            thrSender.Abort();
        }

        // Called when a new customer is accepted
        private void AcceptClient()
        {

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Gamers));

            Set printData = new Set();

            tcpStream = tcpClient.GetStream();
            byte[] bytes = new byte[8000];

            try
            {
                Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    int bytesRead = tcpStream.Read(bytes, 0, bytes.Length);
                }));
            }
            catch { }

            // Checking if I cannot serialize what has come means not to me
            try
            {

                Gamers gamerTest = Json.ReadClient(bytes);
                if (gamerTest == null)
                {
                    // Removes a player when it exits.
                    Host.RemoveUser(tcpClient);
                    return;
                }

            }

            catch { }

            Gamers gamer = Json.ReadClient(bytes);

            // If your ip is not defined, define it
            if (!Host.getMyIp)
            {
                // UDP port FROM FILE for listening from all IPs
                int portUdp = 0;
                Int32.TryParse(Data.SettingCh.St.UdpPort, out portUdp);

                IPEndPoint RemoteEndPoint = null;

                if (gamer.Message != "tcptest" && gamer.Message != null)
                {
                    IPAddress IpHost = IPAddress.Parse(gamer.Message);
                    RemoteEndPoint = new IPEndPoint(IpHost, portUdp);
                    Data.GamersList.gamer[0].Udp = RemoteEndPoint;

                    // If you have determined your IP, then set true
                    Host.getMyIp = true;
                }
                else
                {
                    Set message = new Set();
                    message.SetText("TCP port is open");
                }

            }

            // Objects for serialization
            GamerList gamerList = new GamerList();

            //________________________________ Parsing messages _________________________________\\

            // Initialization message
            if (gamer.Type == 0)
            {
                // If the room is closed, I inform this to the person who sent the request and disconnect the connection

                if (Data.LobbyOpen == false)
                {

                    gamerList.Type = -3;

                    tcpStream.Write(Json.WriteHost(gamerList), 0, Json.WriteHost(gamerList).Length);
                    tcpStream.Flush();
                    CloseConnection();
                    return;
                }


                if (Data.UseLobbyPassword == true)
                {
                    if (gamer.Key != "yes")
                    {
                        if (gamer.Password != Data.SettingCh.St.Password)
                        {
                            gamerList.Type = -2;

                            tcpStream.Write(Json.WriteHost(gamerList), 0, Json.WriteHost(gamerList).Length);
                            tcpStream.Flush();
                            CloseConnection();
                            return;
                        }
                    }
                }

                if (gamer.Name != "" && gamer.Name != null)
                {

                    // Loop through all the names         
                    for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                    {
                        if (gamer.Name == Data.GamersList.gamer[i].Name)
                        {

                            gamerList.Type = -1;

                            tcpStream.Write(Json.WriteHost(gamerList), 0, Json.WriteHost(gamerList).Length);
                            tcpStream.Flush();
                            CloseConnection();
                            return;

                        }

                    }
                }

                else
                {
                    gamerList.Type = -4;

                    tcpStream.Write(Json.WriteHost(gamerList), 0, Json.WriteHost(gamerList).Length);
                    tcpStream.Flush();
                    CloseConnection();
                    return;

                }

                // Add the user to the lists and start listening to his messages
                Host.tcpClients.Add(tcpClient);
                Host.AddUser(gamer);

                //________________________________ Parsing other messages _________________________________\\

                try
                {

                    srReceiver = new System.IO.StreamReader(tcpStream);
                    // Peek () here serves as an indicator of client disconnection
                    int buf = 0;
                    Array.Clear(bytes, 0, bytes.Length);

                    // Waiting for a message from the player
                    while ((buf = tcpStream.Read(bytes, 0, bytes.Length)) != 0 || srReceiver.Peek() == -1)

                    {

                        Set printDat = new Set();

                        gamer = Json.ReadClient(bytes);

                        // Change of car
                        if (gamer.Type == 1)
                        {
                            Data.GamersList.gamer[gamer.Number].GCar = gamer.Car;
                            GamerList gamerListCar = new GamerList();
                            gamerListCar.Type = 2;
                            gamerListCar.Number = gamer.Number;
                            gamerListCar.Map = gamer.Car;

                            Host.SendAllThread(gamerListCar);
                        }

                        // Team change
                        if (gamer.Type == 2)
                        {
                            Data.GamersList.gamer[gamer.Number].GTeam = gamer.Team;
                            GamerList gamerListTeam = new GamerList();
                            gamerListTeam.Type = 11;
                            gamerListTeam.Number = gamer.Number;
                            gamerListTeam.Map = gamer.Team;

                            Host.SendAllThread(gamerListTeam);
                        }

                        // Changed status
                        if (gamer.Type == 3)
                        {

                            // Change status to Not Ready
                            if (gamer.Status == 0)
                            {
                                // Need "approval" of the Host, when he clicked Ready, you can not reset
                                if (Data.GamersList.gamer[0].Status == 0)
                                {
                                    Data.GamersList.gamer[gamer.Number].GStatus = 0;
                                    GamerList gamerListTeam = new GamerList();
                                    gamerListTeam.Type = 3;
                                    gamerListTeam.Number = gamer.Number;
                                    gamerListTeam.Map = 0;

                                    Host.SendAllThread(gamerListTeam);
                                }

                            }
                            // If the player sent that Ready
                            else
                            {
                                // Sorting through the cycle of all whose status is ready          
                                for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                                {
                                    if (Data.GamersList.gamer[i].Status != 0)
                                    {
                                        if (gamer.Car == Data.GamersList.gamer[i].Car)
                                        {
                                            gamer.Status = 0;
                                            GamerList gamerListMap = new GamerList();
                                            gamerListMap.Type = 5;
                                            gamerListMap.Number = i;

                                            Host.SendToNumberThread(gamer.Number, gamerListMap);
                                            break;
                                        }
                                    }
                                }

                                Data.GamersList.gamer[gamer.Number].GStatus = gamer.Status;
                                GamerList gamerListTeam = new GamerList();
                                gamerListTeam.Type = 3;
                                gamerListTeam.Number = gamer.Number;
                                gamerListTeam.Map = gamer.Status;

                                Host.SendAllThread(gamerListTeam);
                            }

                            // Control the start button
                            Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                            {
                                Data.LobbyLink.Go();
                            }));

                        }

                        // Message
                        if (gamer.Type == 4)
                        {

                            Set setMassage = new Set();
                            GamerList gamerListMap = new GamerList();
                            gamerListMap.Type = 4;
                            gamerListMap.Number = gamer.Number;
                            gamerListMap.Message = gamer.Message;

                            Host.SendAllThread(gamerListMap);

                            setMassage.SetMassage(gamer.Number, gamer.Message);

                        }

                        // Remove player
                        if (gamer.Type == 7)
                        {
                            Host.RemoveUser(tcpClient);
                        }
                    }
                }

                catch { }

            }
        }


    }
}
