using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO;
using System.Timers;
using System.Drawing;



namespace TMC
{
    class Web
    {

        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer aTimer1;
        private static System.Timers.Timer aTimerList;
        private static System.Timers.Timer aTimerList1;

        // Create a game in a new thread and update with a period
        public void CreateLobbyInThread()
        {

            Thread RoomThread = new Thread(new ThreadStart(CreateLobbyByTimer));
            RoomThread.Start();
        }

        // Create a list of all sessions in a new thread and update with a period on the main form
        public void CreateListThread()
        {
            Thread RoomsThread = new Thread(new ThreadStart(CreateListLobbyByTimer));
            RoomsThread.Start();
        }

        // One-time get a list of all the lobby in the new thread
        public void CreateLobbyListOnes()
        {
            Thread RoomsThread = new Thread(new ThreadStart(GetLobbyListOnetime));
            RoomsThread.Start();

        }

        // Request in a new thread to delete his game from the server
        public void DeleteLobbyThread()
        {
            Thread GetThread = new Thread(new ThreadStart(DeleteLobby));
            GetThread.Start();
        }

        // Make changes to the created game, applied in a new thread
        public void UpdLobbyThread()
        {
            Thread UpdThread = new Thread(new ThreadStart(UpdateLobby));
            UpdThread.Start();
        }

        // Send a request for a list to the server address in the settings
        public string SendToServer(string command)
        {
            WebRequest request = null;
            try
            {
                if (Data.SettingCh.St.Server != "" && Data.SettingCh.St.Server != null)
                {
                    request = WebRequest.Create(Data.SettingCh.St.Server + "?" + command);
                    request.Proxy = new WebProxy();
                    request.Timeout = 8000;


                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string s = reader.ReadToEnd();
                                return s;
                            }
                        }
                    }
                }
                else
                {
                    Set mas = new Set();
                    mas.SetSomeInfo("Error connection with server. The server address may be incorrect \r\n");

                }
            }
            catch (WebException ex)
            {
                // Get exception status
                WebExceptionStatus status = ex.Status;

                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    return (httpResponse.StatusCode).ToString();

                }
            }

            return null;
        }

        // Request to remove the game from the server
        public void DeleteLobby()
        {
            Web send = new Web();

            try
            {
                send.SendToServer("cmd=del&name=" + Data.NameSession + "&port=" + Data.SettingCh.St.TcpPort);
            }
            catch { }
        }

        // Create a line with all the parameters that we will send in the request is not the server
        public string CreateLobby()
        {
            Get get = new Get();
            Web web = new Web();
            Crypt cry = new Crypt();

            string cry_tcp = cry.cry(Data.SettingCh.St.TcpPort);

            string create = "cmd=set&name=" + Data.NameSession + "&port=" + Data.SettingCh.St.TcpPort + "&pass=" + PassStat() + ":" + Data.GamersList.IdSession;
            create += "&nop=" + Data.GamersList.gamer.Count + "&pnames=" + web.TakeNames();
            create += "&map=" + get.GetMapToIndex(Data.SettingCh.St.Map) + "&score=" + Data.GamersList.Score;
            create += "&type=" + get.GetTypeToIndex(Data.SettingCh.St.TypeGame) + ":" + Data.ModOrOrigin + "&status=" + StatusRoom() + ":" + Data.Comment;

            return web.SendToServer(create);

        }

        // Lobby tools

        // Make changes to the created game, applied in parallel with the update flow
        public void UpdateLobby()
        {
            Web web = new Web();

            if (web.CreateLobby() == "err")
            {

            }

        }

        // We get a string of all player names through ";", for sending to the server
        public string TakeNames()
        {
            Get usern = new Get();

            string nameline = "";

            for (int i = 0; i < Data.GamersList.gamer.Count; i++)
            {
                nameline += Data.GamersList.gamer[i].Name + ";";

            }

            if (nameline != "")
                return nameline = nameline.Substring(0, nameline.Length - 1);
            else
                return "";

        }

        public static string PassStat()
        {
            if (Data.UseLobbyPassword == false || Data.Password == "" || Data.Password == null)
                return "No";
            else
                return "Yes";


        }

        // Get the status of the game to send to the server
        public string StatusRoom()
        {
            if (Data.LobbyOpen == false)
            {
                return "Close";
            }

            else
            {
                return "Open";
            }
        }

        // Create a game on the server and update it with a certain period (10sec) in a cycle
        public void CreateLobbyByTimer()
        {
            Web _web = new Web();
            _web.CreateLobby();

            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            // Create a timer with a two second interval.
            aTimer1 = new System.Timers.Timer(500);
            // Hook up the Elapsed event for the timer. 
            aTimer1.Elapsed += OnTimedEventStop;
            aTimer1.AutoReset = true;
            aTimer1.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            Web web = new Web();
            if (web.CreateLobby() != "ok")
            {
                //Set.Setinf("Не получилось создать/обновить лобби");
            }

        }

        private void OnTimedEventStop(Object source, ElapsedEventArgs e)
        {
            if (Data.FollowToLobby == false)
            {
                aTimer1.Stop();
                aTimer1.Dispose();
                aTimer1.Enabled = false;
                aTimer.Stop();
                aTimer.Dispose();
                aTimer.Enabled = false;

            }
        }


        // Get a list of all the players in the room
        public List<string> LobbyPlayers(string line)
        {
            string[] pars;
            List<string> list = new List<string>();

            if (line != null && line != "")
            {
                // Divide the string into parts by name
                pars = line.Split(';');
                foreach (var _part in pars)
                {
                    // Fill the list with names
                    list.Add(_part);
                }

                return list;

            }
            else
            {
                list.Clear();
                return list;
            }
        }

        public string GetMaxCountGamers(string _type, int _count)
        {
            switch (_type)
            {
                case "FFA":
                    return _count.ToString();
                case "1x1":
                    return _count.ToString() + "/2";
                case "2x2":
                    return _count.ToString() + "/4";
                case "3x3":
                    return _count.ToString() + "/6";
                case "4x4":
                    return _count.ToString() + "/8";

                default:
                    return null;
            }
        }


        // Get the lobby list and fill in the table
        public string GetLobbyList()
        {
            Set printData = new Set();
            string serveranswer = "";
            string[] line;
            string[] pars;
            int numRoom = 0;
            List<Lobby> LobbySortList = new List<Lobby>();

            try
            {
                // Send a request to the server
                serveranswer = SendToServer("cmd=get");


                if (serveranswer != null && serveranswer != "err" && serveranswer != "")
                {


                    int _lastCount = Data.webList.Count;
                    // Share the response by session
                    line = serveranswer.Split('&');
                    try
                    {

                        Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                        {

                            // Clear the list before filling it
                            LobbySortList.Clear();

                        }));
                    }
                    catch { return "err"; }

                    foreach (var part in line)
                    {
                        if (part != "" && part != null)
                        {

                            // In order not to count from zero, enter the counter from 1
                            numRoom++;

                            // Divide sessions by parameters
                            pars = part.Split('|');
                            Get getType = new Get();

                            // Use this function of receiving through the separator ":"
                            string type = getType.GetIpAndPort(pars[6], 0);
                            string mod = getType.GetIpAndPort(pars[6], 1);

                            // Get a password or not, and IdTime
                            string _pass = getType.GetIpAndPort(pars[3], 0);
                            int _id = 0;
                            Int32.TryParse(getType.GetIpAndPort(pars[3], 1), out _id);

                            string openClose = getType.GetIpAndPort(pars[9], 0);

                            string comment = getType.GetIpAndPort(pars[9], 1);

                            int count = 0;
                            Int32.TryParse(pars[4], out count);

                            try
                            {
                                if (count > 0)
                                {

                                    Data.LobbyList.Dispatcher.Invoke(new Action(delegate ()
                                    {
                                        LobbySortList.Add(new Lobby()
                                        {
                                            Name = pars[0],
                                            Host = LobbyPlayers(pars[5])[0],
                                            Players = LobbyPlayers(pars[5]),
                                            Map = pars[7],
                                            Type = type,
                                            Password = _pass,
                                            IdTime = _id,
                                            ModOrigin = mod,
                                            OpenCloseIngame = openClose,
                                            Comment = comment,
                                            Score = pars[8],
                                            IP = pars[1],
                                            Port = pars[2],
                                            Count = GetMaxCountGamers(type, count),
                                            CountOne = pars[4]
                                        });

                                    }));
                                }

                            }

                            catch
                            {
                                return "err";
                            }

                        }
                        else
                        {

                            // Erases information
                            Set inf = new Set();
                            inf.SetInfo(0);
                        }
                    }

                    // Start sorting the list, the first sorting by creation date is always there
                    var sortedList = LobbySortList.OrderBy(x => x.IdTime).ToList();

                    Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                    {
                        // Build a list by numbers
                        for (int i = 0; i < sortedList.Count; i++)
                        {
                            sortedList[i].Number = i + 1;
                        }

                        // Before distillation into the table, I clean it
                        Data.webList.Clear();
                        if (!Data.SettingCh.St.Sort1 && !Data.SettingCh.St.Sort2 && !Data.SettingCh.St.Sort3 && !Data.SettingCh.St.Sort4)
                        {
                            for (int i = 0; i < sortedList.Count; i++)
                            {
                                Data.webList.Add(sortedList[i]);
                            }

                        }
                        else
                        if (Data.SettingCh.St.Sort1)
                        {
                            var sortedListMod = sortedList.OrderByDescending(x => x.OpenCloseIngame).ThenBy(x => x.Password).ThenBy(x => x.ModOrigin).ToList();
                            for (int i = 0; i < sortedList.Count; i++)
                            {
                                Data.webList.Add(sortedListMod[i]);
                            }

                        }
                        else
                        if (Data.SettingCh.St.Sort2)
                        {
                            var sortedListOrig = sortedList.OrderByDescending(x => x.OpenCloseIngame).ThenBy(x => x.Password).ThenByDescending(x => x.ModOrigin).ToList();
                            for (int i = 0; i < sortedList.Count; i++)
                            {
                                Data.webList.Add(sortedListOrig[i]);
                            }

                        }
                        else
                        if (Data.SettingCh.St.Sort3)
                        {
                            var sortedListMap = sortedList.OrderByDescending(x => x.OpenCloseIngame).ThenBy(x => x.Password).ThenBy(x => x.Map).ToList();
                            for (int i = 0; i < sortedList.Count; i++)
                            {
                                Data.webList.Add(sortedListMap[i]);
                            }

                        }
                        if (Data.SettingCh.St.Sort4)
                        {
                            var sortedListType = sortedList.OrderByDescending(x => x.OpenCloseIngame).ThenBy(x => x.Password).ThenBy(x => x.Type).ToList();
                            for (int i = 0; i < sortedList.Count; i++)
                            {
                                Data.webList.Add(sortedListType[i]);
                            }

                        }

                    }));

                    return "ok";
                }
                else
                {
                    Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                    {
                        // Clear the list
                        LobbySortList.Clear();
                        Data.webList.Clear();

                        // Erases information
                        Set inf = new Set();
                        if (serveranswer == null)
                            inf.SetSomeInfo("Server is not responding. Check if the server is specified correctly in the settings \r\n");
                        if (serveranswer == "")
                            inf.SetSomeInfo("No lobby found \r\n");
                        if (serveranswer == "err")
                            inf.SetSomeInfo("Server is not responding. Check if the server is specified correctly in the settings \r\n");
                    }));

                    return "ok";
                }

            }
            catch
            {
                if (Data.FollowToGetLobby)
                {
                    Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                    {

                        // Clear the list
                        LobbySortList.Clear();
                        Data.webList.Clear();

                        // Erases information
                        Set inf = new Set();
                        inf.SetInfo(0);

                        inf.SetSomeInfo("Server is not responding. Check if the server is specified correctly in the settings \r\n");
                    }));

                }

                return "err";
            }
        }

        // Get the lobby list and fill the table once
        public void GetLobbyListOnetime()
        {
            Web _getList = new Web();
            GetLobbyList();
        }

        // Create a list of sessions in a cycle of (10 seconds), to terminate Data.FollowToGetLobby = false;
        public void CreateListLobbyByTimer()
        {

            GetLobbyListOnetime();

            // Create a timer with a two second interval.
            aTimerList = new System.Timers.Timer(15000);
            // Hook up the Elapsed event for the timer. 
            aTimerList.Elapsed += OnTimedEventList;
            aTimerList.AutoReset = true;
            aTimerList.Enabled = true;

            // Create a timer with a two second interval.
            aTimerList1 = new System.Timers.Timer(300);
            // Hook up the Elapsed event for the timer. 
            aTimerList1.Elapsed += OnTimedEventOffList;
            aTimerList1.AutoReset = true;
            aTimerList1.Enabled = true;

        }

        public void OnTimedEventList(object sender, EventArgs e)
        {
            GetLobbyListOnetime();
        }

        public void OnTimedEventOffList(object sender, EventArgs e)
        {
            if (Data.FollowToGetLobby == false)
            {

                aTimerList1.Stop();
                aTimerList1.Dispose();
                aTimerList.Stop();
                aTimerList.Dispose();

            }
        }

    }
}
