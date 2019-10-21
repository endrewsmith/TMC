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

namespace TMC
{
    public class Get
    {
        public string GetMapToIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return "Los Angeles";
                case 1:
                    return "Moscow";
                case 2:
                    return "Paris";
                case 3:
                    return "Amazonia";
                case 4:
                    return "New York";
                case 5:
                    return "Antarctica";
                case 6:
                    return "Holland";
                case 7:
                    return "Hong Kong";
                case 8:
                    return "Suicide Swamp";
                case 9:
                    return "Assault on Cyburbia";
                case 10:
                    return "Rooftop Combat";
                default:
                    return "";
            }
        }

        public string GetCarToIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return "Hammerhead";
                case 1:
                    return "Outlaw 2";
                case 2:
                    return "Warthog";
                case 3:
                    return "Mr. Grimm";
                case 4:
                    return "Grasshopper";
                case 5:
                    return "Thumper";
                case 6:
                    return "Spectre";
                case 7:
                    return "Roadkill";
                case 8:
                    return "Twister";
                case 9:
                    return "Axel";
                case 10:
                    return "Mr. Slam";
                case 11:
                    return "Shadow";
                case 12:
                    return "Sweet Tooth";
                case 13:
                    return "Minion";

                default:
                    return "";
            }
        }

        public string GetTypeToIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return "FFA";
                case 1:
                    return "1x1";
                case 2:
                    return "2x2";
                case 3:
                    return "3x3";
                case 4:
                    return "4x4";

                default:
                    return "";
            }
        }

        public int GetMaxCountGamers(int index)
        {
            switch (index)
            {
                case 0:
                    return 8;
                case 1:
                    return 2;
                case 2:
                    return 4;
                case 3:
                    return 6;
                case 4:
                    return 8;

                default:
                    return 0;

            }

        }

        public string GetColorTeam(int index)
        {

            switch (index)
            {
                case 1:
                    return "#c6ffa7";
                case 2:
                    return "#ffcba9";
                case 3:
                    return "#d4f1ec";
                case 4:
                    return "#c7f2d1";
                case 5:
                    return "#75c8cc";
                case 6:
                    return "#fcfdb1";
                case 7:
                    return "#e4c5e4";
                case 8:
                    return "#fcdcdf";

                default:
                    return "#ffffff";

            }

        }

        public string GetColorGamer(int index)
        {

            switch (index)
            {
                case 0:
                    return "#179420";
                case 1:
                    return "#a52918";
                case 2:
                    return "#296fa0";
                case 3:
                    return "#923bb6";
                case 4:
                    return "#509da8";
                case 5:
                    return "#8c7f2a";
                case 6:
                    return "#f02a70";
                case 7:
                    return "#4e4ef0";

                default:
                    return "#ffffff";

            }

        }

        public void GetMyNamber()
        {
            for (int i = 0; i < Data.GamersList.gamer.Count; i++)
            {
                if (Data.GamersList.gamer[i].Name == Data.SettingCh.St.Name)
                    Data.MyNamber = i;

            }
        }
        // Shows readiness if it returned true then everyone is ready
        public bool GetReady()
        {
            // Sorted through all in a loop if someone is not ready returned false
            for (int i = 0; i < Data.GamersList.gamer.Count; i++)
            {
                if (Data.GamersList.gamer[i].Status == 0)
                    return false;
            }

            // If everyone is ready true
            return true;
        }

        // Shows readiness if it returned true then everyone is ready
        public bool GetReadyPing()
        {
            try
            {

                // Went through all in a loop, if someone is not ready, returned false
                for (int i = 0; i < Data.GamersList.gamer.Count; i++)
                {
                    if (Data.MyNamber != i && Data.GamersList.gamer[i] != null)
                    {
                        if (Data.GamersList.gamer[i].GPing == "" || Data.GamersList.gamer[i].GPing == null)
                            return false;
                    }

                }
                return true;
            }
            catch { return false; }

        }

        // Compare the number of gamers with the maximum possible, if that I close the room or open
        public void GetClosingLobby()
        {

            if (Data.GamersList.gamer.Count >= Data.MaxGamers)
            {
                Data.LobbyLink.LockLobby();
            }
            else
            {
                Data.LobbyLink.UnLockLobby();
            }

        }

        // Get your external ip from internet
        public string GetPublicIP()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                request.Proxy = new WebProxy();
                request.Timeout = 10000;
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {

                            string respon = reader.ReadToEnd().Trim();
                            string[] a = respon.Split(':');
                            string a2 = a[1].Substring(1);
                            string[] a3 = a2.Split('<');
                            string a4 = a3[0];
                            return a4;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                return null;
            }

        }

        public string GetIpAndPort(string _ipAndPort, int _index)
        {
            string[] line = _ipAndPort.Split(':');
            switch (_index)
            {
                case 0:
                    return line[0];
                case 1:
                    return line[1];
                default:
                    return null;
            }
        }

        // Removes service characters from a string
        public string hasSpecialChar(string input)
        {
            if (input != null && input != "")
            {
                input = new string(input.Where(e => char.IsLetter(e) && char.ToUpper(e) >= 65 && char.ToUpper(e) <= 90 || char.ToUpper(e) == 95 || char.ToUpper(e) >= 48 && char.ToUpper(e) <= 57).ToArray());

                if (input.Length > 10)
                {
                    input = input.Substring(0, 10);
                }
            }

            return input;
        }

        // Removes service characters from a string and trims the string to the maximum specified size
        public string DelSpecialChar(string input, int max)
        {
            if (input != null && input != "")
            {
                string specialChar = "|&$#";

                foreach (char item in specialChar)
                {
                    if (input.IndexOf(item) != -1)
                    {
                        input = input.Replace(item.ToString(), "");
                    }

                }

                if (input.Length > max)
                {
                    input = input.Substring(0, max);
                }
            }

            return input;
        }

    }
}
