using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TMC
{
    /// <summary>
    /// Логика взаимодействия для SettingsWin.xaml
    /// </summary>
    public partial class SettingsWin : Window
    {
        private void WriteSetting()
        {
            Data.SettingCh.St.Settings_X_pos = this.Left;
            Data.SettingCh.St.Settings_Y_pos = this.Top;
            Data.SettingCh.St.Settings_Width = this.Width;
            Data.SettingCh.St.Settings_Height = this.Height;
            if (WindowState == WindowState.Maximized)
            {
                Data.SettingCh.St.SettingsMaximized = true;
            }
            else
            {
                Data.SettingCh.St.SettingsMaximized = false;
            }
            Get name = new Get();

            // Server link 
            Data.SettingCh.St.Server = ServerInput.Text;
            // ld.exe path
            Data.SettingCh.St.LdPath = LdexeInput.Text;
            if (NameInput.Text == "")
            {
                Data.SettingCh.St.Name = "TM2_Player"; // Gamer name
            }
            else
            {
                Data.SettingCh.St.Name = name.hasSpecialChar(NameInput.Text); 
            }


            Data.SettingCh.St.TcpPort = TCPInput.Text;  // tcp port
            Data.SettingCh.St.UdpPort = UDPInput.Text;  // udp port
            Data.SettingCh.St.IP = IPInput.Text;        // ip

            if ((bool)UseIpStatic.IsChecked)
            {
                Data.SettingCh.St.UseAutoIP = true;
            }
            else
            {
                Data.SettingCh.St.UseAutoIP = false;
            }

            Data.SettingCh.WriteXml();
        }

        // Reading settings
        private void ReadSetting()
        {
            GameSettings getName = new GameSettings();

            Data.SettingCh.ReadXml();

            this.Left = Data.SettingCh.St.Settings_X_pos;
            this.Top = Data.SettingCh.St.Settings_Y_pos;
            this.Width = Data.SettingCh.St.Settings_Width;
            this.Height = Data.SettingCh.St.Settings_Height;
            if (Data.SettingCh.St.SettingsMaximized)
            {
                WindowState = WindowState.Maximized;
            }

            Get name = new Get();
            // Server link 
            ServerInput.Text = Data.SettingCh.St.Server;
            // ld.exe path
            LdexeInput.Text = Data.SettingCh.St.LdPath; 

            TCPInput.Text = Data.SettingCh.St.TcpPort;  // tcp port
            UDPInput.Text = Data.SettingCh.St.UdpPort;  // udp port
            IPInput.Text = Data.SettingCh.St.IP;        // ip 

            if (Data.SettingCh.St.UseAutoIP == true)
            {               
                UseIpStatic.IsChecked = true;
            }
            else
            {
                UseIpStatic.IsChecked = false;
            }

            if (!Data.SettingCh.St.UseLocalModule)
            {
                MainGrid.RowDefinitions[7].Height = new GridLength(0, GridUnitType.Pixel);
                MainGrid.RowDefinitions[8].Height = new GridLength(0, GridUnitType.Pixel);
            }

            string pathLoader = "";
            if (Data.SettingCh.St.LdPath != null)
            {
                pathLoader = Path.Combine(Data.SettingCh.St.LdPath, "loader.cfg");
            }

            if (File.Exists(pathLoader))
            {
                
                string getMyName = getName.ReadParametrConfig("Name");
               
                if (getMyName != null && getMyName != "" && getMyName.Length != 0)
                {
                    
                    NameInput.Text = getName.ReadParametrConfig("Name");

                }
                    
                else if(Data.SettingCh.St.Name != null || Data.SettingCh.St.Name != "")
                    NameInput.Text = Data.SettingCh.St.Name;
                else
                    NameInput.Text = "TM2_Player";
            }
            else
            {
                if (Data.SettingCh.St.Name != null && Data.SettingCh.St.Name != "")
                    NameInput.Text = Data.SettingCh.St.Name;
                else
                    NameInput.Text = "TM2_Player"; 
            }
        }

        public SettingsWin()
        {
            InitializeComponent();
            ReadSetting();
        }

        private void ServerInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ServerInput.ActualHeight > 0)
                ServerInput.FontSize = ServerInput.ActualHeight / (1.6);
        }
        private void LdexeInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LdexeInput.ActualHeight > 0)
                LdexeInput.FontSize = LdexeInput.ActualHeight / (1.6);
        }

        private void NameInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (NameInput.ActualHeight > 0)
                NameInput.FontSize = NameInput.ActualHeight / (1.6);
        }

        private void TCPInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TCPInput.ActualHeight > 0)
                TCPInput.FontSize = TCPInput.ActualHeight / (1.6);
        }

        private void UDPInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (UDPInput.ActualHeight > 0)
                UDPInput.FontSize = UDPInput.ActualHeight / (1.6);
        }

        private void IPInput_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IPInput.ActualHeight > 0)
                IPInput.FontSize = IPInput.ActualHeight / (1.6);
        }


        private void LdBtnPaht_Click(object sender, RoutedEventArgs e)
        {

            // Find the file from the explorer and write off the path to it
            string filePath = "";

            OpenFileDialog tm2exe = new OpenFileDialog();
            tm2exe.Filter = "File .exe|ld.exe";
            tm2exe.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory.ToString();
            if (tm2exe.ShowDialog() == true)
            {
                // File path
                FileInfo fileInf = new FileInfo(tm2exe.FileName);
                filePath = fileInf.DirectoryName + "\\";
                LdexeInput.Text = filePath;
            }

            string pathLoader = "";
            if (filePath != null)
            {
                pathLoader = Path.Combine(filePath, "loader.cfg");
            }

            if (File.Exists(pathLoader))
            {
                Data.SettingCh.St.LdPath = filePath;
                GameSettings name = new GameSettings();
                string getName = name.ReadParametrConfig("Name");
                if(getName != null || getName != "")
                NameInput.Text = name.ReadParametrConfig("Name");
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {

            WriteSetting();

            Data.MainWin.CheckIp();

            Data.MainWin.CheckServerIpPorts();

            GameSettings saveName = new GameSettings();
            
            saveName.ChangeConfig(Data.SettingCh.St.Name, "Name", null);
            
            Data.MainWin.LockGameSettings();

        }

        private void UseIpStatic_Checked(object sender, RoutedEventArgs e)
        {
            
            IPInput.IsEnabled = false;
            GetIPThread();
        }
        // get your external ip from internet in new thead
        public void GetIPThread()
        {
            Thread GetIPThread = new Thread(new ThreadStart(GetIPAndSet));
            GetIPThread.Start();
        }
        // Get your external ip from internet
        public void GetIPAndSet()
        {
            Get getIp = new Get();
            string ip = getIp.GetPublicIP();
            try
            {
                if (ip != null)
                {
                    Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                    {
                        IPInput.Text = ip;
                        Data.MainWin.YourIp.Text = Data.SettingCh.St.IP;

                    }));


                    Data.SettingCh.St.IP = ip;
                }
                else
                {
                    Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                    {
                        UseIpStatic.IsChecked = false;
                        IPInput.IsEnabled = true;
                        IPInput.Tag = "automatic detection does not work, enter manually";

                    }));

                }
            }
            catch { }

            Data.MainWin.CheckServerIpPorts();

        }

        private void UseIpStatic_Unchecked(object sender, RoutedEventArgs e)
        {
            IPInput.IsEnabled = true;
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameCount.Text = (10 - NameInput.Text.Length).ToString(CultureInfo.InvariantCulture);

            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z_]", "");
            textboxSender.SelectionStart = cursorPosition;
        }

        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
               return true;
            if (c >= 'A' && c <= 'z')
               return true;
            //if (c == 34)
            //    return false;
            return false;
        }

        private void NameInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((NameInput.Text.Length > 9) && (e.Key.ToString() != "Back") && (e.Key.ToString() != "Delete")
                && (e.Key.ToString() != "Left") && (e.Key.ToString() != "Right"))
            {
                e.Handled = true;
            }
        }

        private void LdexeInput_LostFocus(object sender, RoutedEventArgs e)
        {
            
            
            string pathLoader = "";
           
            Data.SettingCh.St.LdPath = LdexeInput.Text;
            
            if (Data.SettingCh.St.LdPath != null)
            {
                pathLoader = Path.Combine(Data.SettingCh.St.LdPath, "loader.cfg");


                if (File.Exists(pathLoader))
                {
                    GameSettings getName = new GameSettings();
                    string getMyName = getName.ReadParametrConfig("Name");
                    if (getMyName != null || getMyName != "")
                        NameInput.Text = getName.ReadParametrConfig("Name");

                }
            }

        }


    }
}
