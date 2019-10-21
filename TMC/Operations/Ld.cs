using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using WpfAnimatedGif;


using System.Windows;
using System.Windows.Threading;

namespace TMC
{
    class Ld
    {

        // sending data to ld and its launch 

        public void StartLd()

        {
            Get par = new Get();
            string cParams = "";
            Host close = new Host();
            close.StopListening();

            // If the server is running, I send a command to all clients to start ld
            if (Data.HostOrClient)
            {
                Data.GamersList.Type = 8;
                Host.SendAllThread(Data.GamersList);
            }

            // Complete constant ping
            Data.UdpWorkSend = false;

            // Compose a string of parameters that I will pass to ld             

            // Game id
            cParams = "-i " + Data.GamersList.IdSession;

            // Game mode
            cParams += " -g 0";

            // Game mode, if any, then the original
            if (Data.GamersList.ModOrig != "mod")
            {
                cParams += " -c";
            }
                       
            // Map
            cParams += " -m " + Data.GamersList.Map;

            // Player ip: UDP port
            if (Data.HostOrClient)
            {
                cParams += " -l " + "127.0.0.1" + ":" + Data.SettingCh.St.UdpPort;
            }
            else
            {
                cParams += " -l " + "127.0.0.1" + ":" + Data.GamersList.gamer[Data.MyNamber].Udp.Port.ToString();
            }
            
           
            // Player machine: team
            cParams += " -p " + Data.GamersList.gamer[Data.MyNamber].Car + ":" + Data.GamersList.gamer[Data.MyNamber].Team;

            // Data of other players
            for (int i = 0; i < Data.GamersList.gamer.Count; i++)
            {

                // Sorting out all but myself
                if (Data.MyNamber != i)
                {
                    cParams += " -e " + Data.GamersList.gamer[i].Udp.Address + ":" + Data.GamersList.gamer[i].Udp.Port.ToString();
                    cParams += ":" + Data.GamersList.gamer[i].Car + ":" + Data.GamersList.gamer[i].Team + ":" + Data.GamersList.gamer[i].Name;
                }

            }

            // For synchronization
            cParams += " -n";

            // Score until how much will the game
            if (Data.GamersList.gamer.Count > 1)
                cParams += " -s " + Data.GamersList.Score;
            else
                cParams += " -s " + "0";

            Encoding utf = Encoding.ASCII;
            Encoding win = Encoding.GetEncoding(1251);

            byte[] utfArr = utf.GetBytes(cParams);
            byte[] winArr = Encoding.Convert(win, utf, utfArr);

            // Try to open ld.exe at the address specified in the settings
            string filename = Path.Combine(Data.SettingCh.St.LdPath, "ld.exe");

            if (File.Exists(filename))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(filename);
                startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
                startInfo.Arguments = cParams;

                Process proc = new Process();
                proc.StartInfo = startInfo;
                proc.Start();

                // Start the removal of the lobby from the server
                Data.FollowToLobby = false;
                Web delateLobby = new Web();
                delateLobby.DeleteLobbyThread();

                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {

                    Data.LobbyLink.LockLobbyLd();
                    // If the module with pictures is enabled
                    if (!Data.SettingCh.St.UseBottomhModule)
                    {
                        // Clear image resources

                        if (Data.HostOrClient)
                        {
                            Data.LobbyLink.ClearImg();
                        }
                        else
                        {
                            Data.LobbyLink.ClearImg();
                        }

                    }
                 
                }));
                
                proc.WaitForExit();

                Data.LobbyLink.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.LobbyLink.LobbyClose();
                    Data.Inf.Focus();
                }));

            }

            // If it didn’t work open, we display warnings on the forms
            else
            {
                Set printData = new Set();
                printData.SetText("Failed to open ld.exe");
            }
        }

        // Start pinging in a new thread
        public void StartLDThread()

        {
            Thread sendThread = new Thread(new ThreadStart(StartLd));
            sendThread.Start();
        }

    }
}
