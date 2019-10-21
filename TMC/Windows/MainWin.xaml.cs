using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.IO;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using WpfAnimatedGif;
using System.Timers;
using System.Text.RegularExpressions;

namespace TMC
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    /// 


    public partial class MainWin : Window
    {
        private static BitmapImage _image;
        private static System.Timers.Timer _timerUpdate;
        private static System.Timers.Timer _timerUpdateStop;
        private bool Updater;


        // Recording Settings
        private void WriteSetting()
        {
            Data.SettingCh.St.TMC_X_pos = this.Left;
            Data.SettingCh.St.TMC_Y_pos = this.Top;
            Data.SettingCh.St.TMC_Width = this.Width;
            Data.SettingCh.St.TMC_Height = this.Height;
            if (WindowState == WindowState.Maximized)
            {
                Data.SettingCh.St.Maximized = true;
            }
            else
            {
                Data.SettingCh.St.Maximized = false;
            }

            Data.SettingCh.St.MenuGridHeight = MainGrid.RowDefinitions[0].ActualHeight;
            Data.SettingCh.St.InfoGridHeight = WebGridInfo.RowDefinitions[2].ActualHeight;

            Data.SettingCh.St.NameLobby = LobbyName.Text;
            if (ModOrig.Text == "orig")
            {
                Data.SettingCh.St.UseMod = false;
            }
            else
            {
                Data.SettingCh.St.UseMod = true;
            }

            Data.SettingCh.St.TypeGame = LobbyType.SelectedIndex;
            if (PassUse.IsChecked == true)
            {
                Data.SettingCh.St.UsePassword = true;
            }
            else
            {
                Data.SettingCh.St.UsePassword = false;
            }
            Data.SettingCh.St.Password = PasswInput.Text;
            Data.SettingCh.St.Comment = CommentInput.Text;

            Data.SettingCh.WriteXml();
        }

        // Reading settings
        private void ReadSetting()
        {
            Data.SettingCh.St.XMLFileName = Environment.CurrentDirectory + "\\settings.xml";
            Data.SettingCh.ReadXml();

            // Overwrite new file path
            Data.SettingCh.St.XMLFileName = Environment.CurrentDirectory + "\\settings.xml";
            Data.SettingCh.WriteXml();
            this.Left = Data.SettingCh.St.TMC_X_pos;
            this.Top = Data.SettingCh.St.TMC_Y_pos;
            this.Width = Data.SettingCh.St.TMC_Width;
            this.Height = Data.SettingCh.St.TMC_Height;


            if (Data.SettingCh.St.ShowLobbySetting)
            {
                LobSettings.Visibility = Visibility.Visible;
            }
            else
            {
                LobSettings.Visibility = Visibility.Collapsed;
            }

            if (Data.SettingCh.St.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            if (Data.SettingCh.St.UseTopMenu)
            {
                MainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.MenuGridHeight, GridUnitType.Pixel);
            }
            else
            {
                MainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
            }
            MainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.MenuGridHeight, GridUnitType.Pixel);

            if (!Data.SettingCh.St.UseLocalModule)
            {
                MainGrid.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
            }

            WebGridInfo.RowDefinitions[2].Height = new GridLength(Data.SettingCh.St.InfoGridHeight, GridUnitType.Pixel);

            var bc = new BrushConverter();
            if (Data.SettingCh.St.UseMod == false)
            {
                ModOrig.Text = "orig";
                ModOrigGrid.Background = (Brush)bc.ConvertFrom("#9bcfff");

            }
            else
            {
                ModOrig.Text = "mod";
                ModOrigGrid.Background = (Brush)bc.ConvertFrom("#bcf199");
            }

            LobbyType.SelectedIndex = Data.SettingCh.St.TypeGame;

            if (Data.SettingCh.St.UsePassword)
            {
                PassUse.IsChecked = true;
                PasswInput.IsEnabled = true;
            }
            else
            {
                PassUse.IsChecked = false;
                PasswInput.IsEnabled = false;
            }

            Get check = new Get();

            LobbyName.Text = check.DelSpecialChar(Data.SettingCh.St.NameLobby, 30);
            PasswInput.Text = Data.SettingCh.St.Password;
            CommentInput.Text = check.DelSpecialChar(Data.SettingCh.St.Comment, 200);
            KeyInput.Text = Data.SettingCh.St.Key;

            if (!Data.SettingCh.St.Sort1 && !Data.SettingCh.St.Sort2 && !Data.SettingCh.St.Sort3 && !Data.SettingCh.St.Sort4)
            {
                Sort1.IsChecked = false;
                Sort2.IsChecked = false;
                Sort3.IsChecked = false;
                Sort4.IsChecked = false;
            }
            else
            if (Data.SettingCh.St.Sort1)
            {
                Sort1.IsChecked = true;
                Sort2.IsChecked = false;
                Sort3.IsChecked = false;
                Sort4.IsChecked = false;
            }
            else
            if (Data.SettingCh.St.Sort2)
            {
                Sort1.IsChecked = false;
                Sort2.IsChecked = true;
                Sort3.IsChecked = false;
                Sort4.IsChecked = false;
            }
            else
            if (Data.SettingCh.St.Sort3)
            {
                Sort1.IsChecked = false;
                Sort2.IsChecked = false;
                Sort3.IsChecked = true;
                Sort4.IsChecked = false;
            }
            else
            if (Data.SettingCh.St.Sort4)
            {
                Sort1.IsChecked = false;
                Sort2.IsChecked = false;
                Sort3.IsChecked = false;
                Sort4.IsChecked = true;
            }

        }

        // Get from internet your external ip in a new thread
        public void GetIPThread()
        {
            Thread GetIPThread = new Thread(new ThreadStart(GetIPAndSet));
            GetIPThread.Start();
        }

        // Get your external ip from the internet
        public void GetIPAndSet()
        {
            Get getIp = new Get();
            try
            {

                string ip = getIp.GetPublicIP();
                if (ip != null)
                {
                    if (Data.MainWin != null)
                    {

                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            YourIp.Text = ip;
                            if (Data.SettingCh.St.Server != null || Data.SettingCh.St.Server != "")

                                Create.IsEnabled = true;
                        }));

                        Data.SettingCh.St.IP = ip;
                    }


                }
                else
                {
                    if (Data.MainWin != null)
                    {
                        Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                        {
                            YourIp.Text = "Input your IP for local modul in settings";

                        }));
                    }


                }
            }
            catch { }

        }

        // Lock the game settings until ld is selected
        public void LockGameSettings()
        {
            string pathLoader = "";
            if (Data.SettingCh.St.LdPath != null)
            {
                pathLoader = Path.Combine(Data.SettingCh.St.LdPath, "loader.cfg");
            }

            if (File.Exists(pathLoader))
            {
                Gamesettings.IsEnabled = true;
            }
            else
            {
                Gamesettings.IsEnabled = false;
            }
        }

        // Create a lobby and lock the settings in the main window
        public void CreateLobby()
        {

            try
            {
                LobbyWin Lobby = new LobbyWin();
                Lobby.Owner = Data.MainWin;
                Data.LobbyLink = Lobby;

                if (Data.LobbyTitle != null)
                    Lobby.Title = Data.LobbyTitle;

                Data.LobbyLink.Show();
                Lock();
            }
            catch (Exception ex)
            {
                MainWin.Message("Internal error", "Try to reload the program");
            }

        }

        public void CheckServerIpPorts()
        {
            if (Data.SettingCh.St.Server == null || Data.SettingCh.St.TcpPort == null || Data.SettingCh.St.UdpPort == null
                    || Data.SettingCh.St.Server == "" || Data.SettingCh.St.TcpPort == "" || Data.SettingCh.St.UdpPort == "")
            {
                Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                {
                    CreateToServer.IsEnabled = false;
                    Create.IsEnabled = false;
                }));
            }
            else
            {
                Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                {
                    CreateToServer.IsEnabled = true;
                    Create.IsEnabled = true;
                }));
            }

        }
        public void CheckIp()
        {

            if (Data.SettingCh.St.UseAutoIP == true)
            {
                GetIPThread();
            }
            else
            {
                if (Data.SettingCh.St.IP == null || Data.SettingCh.St.IP == "")
                {
                    YourIp.Text = "no IP";
                }
                else
                {
                    YourIp.Text = Data.SettingCh.St.IP;
                }
            }
        }

        // Point of entry
        public MainWin()
        {
            InitializeComponent();
            ReadSetting();

            Get name = new Get();
            Data.SettingCh.St.Name = name.hasSpecialChar(Data.SettingCh.St.Name); // имя игрока


            Data.MainWin = (MainWin)Application.Current.MainWindow;
            Data.LobbyList = WebDataGrid;
            Data.Inf = Information;
            Data.MainGridBlock = MainGrid;

            LockGameSettings();

            string path = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "settings.gif";
            ImgSettings(path, LobbySettingGrid);

            CheckIp();

            WebDataGrid.ItemsSource = Data.webList;

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "TMC [" + Data.Version + "]";

            CheckServerIpPorts();

            // Start updating the list of games in a new thread
            Web getListLobby = new Web();
            Data.FollowToGetLobby = true;
            getListLobby.CreateListThread();

            string pathUpdate = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "update.gif";
            if (File.Exists(pathUpdate))
            {

                _image = null;
                _image = new BitmapImage();
                _image.BeginInit();
                _image.UriSource = new Uri(pathUpdate);
                _image.EndInit();

                ImageBehavior.SetAnimatedSource(UpdateImg, _image);
                var controller = ImageBehavior.GetAnimationController(UpdateImg);
                controller.Pause();

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Data.FollowToGetLobby = false;
            WriteSetting();
        }



        private void Btn_Create(object sender, RoutedEventArgs e)
        {

            Data.HostOrClient = true;
            Data.CreateToServer = true;

            WriteSetting();

            // Get the value from the combobox to the string
            ComboBoxItem typeItem = (ComboBoxItem)LobbyType.SelectedItem;
            string value = typeItem.Content.ToString();
            string _usePass;
            if ((bool)PassUse.IsChecked == true)
            {
                _usePass = " | Password: " + PasswInput.Text;
            }
            else
            {
                _usePass = " ";
            }

            if (Data.SettingCh.St.UseMod)
                Data.ModOrOrigin = "Mod";
            else
                Data.ModOrOrigin = "Orig";


            Data.Password = PasswInput.Text;

            Get _max = new Get();

            // Assign the name of the lobby
            if (LobbyName.Text != "")
                Data.NameSession = LobbyName.Text;
            else
                Data.NameSession = "Lobby by " + Data.SettingCh.St.Name;


            string name;
            if (Data.NameSession.Length > 16)
            {
                name = Data.NameSession.Substring(0, 16) + "...";

            }
            else
            {
                name = Data.NameSession;
            }

            Data.LobbyTitle = name + " [ " + ModOrig.Text + " | " + typeItem.Content.ToString() + _usePass + "]";

            // Assign a comment
            Data.Comment = CommentInput.Text;

            Data.MaxGamers = _max.GetMaxCountGamers(LobbyType.SelectedIndex);
            Data.GamersList.LobbyType = typeItem.Content.ToString();
            Data.GamersList.ModOrig = ModOrig.Text.ToString();

            Host host = new Host();
            host.StartListening(Data.SettingCh.St.TcpPort);

        }

        private void PassUse_Checked(object sender, RoutedEventArgs e)
        {
            PasswInput.IsEnabled = true;
            Data.SettingCh.St.UsePassword = true;
            Data.UseLobbyPassword = true;

        }

        private void PassUse_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswInput.IsEnabled = false;
            Data.SettingCh.St.UsePassword = false;
            Data.UseLobbyPassword = false;

        }

        private void ModOrigGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var bc = new BrushConverter();
            if (ModOrig.Text == "mod")
            {
                ModOrig.Text = "orig";
                ModOrigGrid.Background = (Brush)bc.ConvertFrom("#9bcfff");

                Data.SettingCh.St.UseMod = false;

            }
            else
            {
                ModOrig.Text = "mod";
                ModOrigGrid.Background = (Brush)bc.ConvertFrom("#bcf199");
                Data.SettingCh.St.UseMod = true;
            }

        }

        private void WebDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            Set _inf = new Set();
            _inf.SetInfo(Data.Selected);
        }

        private void WebDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Lobby path = WebDataGrid.SelectedItem as Lobby;
            int parse = WebDataGrid.SelectedIndex;
            Set _inf = new Set();
            _inf.SetInfo(parse);
        }

        private void WebDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Data.LobbyStart != true)
            {
                Data.JoinToServer = true;
                Lobby path = WebDataGrid.SelectedItem as Lobby;
                int parse = WebDataGrid.SelectedIndex;
                if (parse > -1)
                {


                    if (Data.webList[parse].Host != Data.SettingCh.St.Name)
                    {


                        Data.HostOrClient = false;
                        Data.KeyForJoin = KeyInput.Text;
                        Data.LobbyNumber = parse;

                        if (!Data.HostOrClient)
                        {
                            // Running client

                            Client client = new Client();
                            Get getIpOrPort = new Get();

                            // Go through the lobby list
                            if (Data.JoinToServer)
                            {

                                int tcpPort = 0;
                                Int32.TryParse(Data.webList[parse].Port, out tcpPort);
                                client.InitializeConnection(Data.webList[parse].IP, tcpPort);
                            }

                        }
                    }

                }

            }

        }

        private void JoinToLobby_Click(object sender, RoutedEventArgs e)
        {
            if (Data.LobbyStart != true)
            {
                Data.JoinToServer = true;
                Lobby path = WebDataGrid.SelectedItem as Lobby;
                int parse = WebDataGrid.SelectedIndex;



                Data.HostOrClient = false;
                Data.KeyForJoin = KeyInput.Text;
                Data.LobbyNumber = parse;

                if (!Data.HostOrClient)
                {
                    // Running ckient

                    Client _client = new Client();
                    Crypt _decrypt = new Crypt();
                    Get _getIpOrPort = new Get();


                    // Entry in the lobby list
                    if (Data.JoinToServer)
                    {

                        int _tcpPort = 0;
                        Int32.TryParse(Data.webList[parse].Port, out _tcpPort);
                        _client.InitializeConnection(Data.webList[parse].IP, _tcpPort);
                    }
                    // entry in the local modul
                    else
                    {
                        string _ipPort = _decrypt.decry(Data.KeyForJoin);
                        int _tcpPort = 0;
                        Int32.TryParse(_getIpOrPort.GetIpAndPort(_ipPort, 1), out _tcpPort);
                        _client.InitializeConnection(_getIpOrPort.GetIpAndPort(_ipPort, 0), _tcpPort);
                    }
                }

            }
        }


        private void CommentInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommentCount.Text = (200 - CommentInput.Text.Length).ToString(CultureInfo.InvariantCulture);
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Zа-яА-Я@!%/()=?»«@£§€{}.;'<>_,# ]", "");
            textboxSender.SelectionStart = cursorPosition;
        }

        private void LobbyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            LobbyNameCount.Text = (30 - LobbyName.Text.Length).ToString(CultureInfo.InvariantCulture);
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Zа-яА-Я@!%/()=?»«@£§€{}.;'<>_,# ]", "");
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
            if (c >= 'А' && c <= 'я')
                return true;
            if (c >= '!' && c <= '@')
                return true;
            if (c == '#' || c == '$')
                return false;
            return false;
        }

        public void Lock()
        {
            MenuGrid.IsEnabled = false;
            LocalGrid.IsEnabled = false;
            WebBtnGrid.IsEnabled = false;

            WebDataGrid.Columns[8].Visibility = Visibility.Collapsed;
            Data.LobbyStart = true;
        }
        public void UnLock()
        {
            MenuGrid.IsEnabled = true;
            LocalGrid.IsEnabled = true;
            WebBtnGrid.IsEnabled = true;

            WebDataGrid.Columns[8].Visibility = Visibility.Visible;
            Data.LobbyStart = false;
        }

        public static void Message(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UpdateLobbyList(object sender, RoutedEventArgs e)
        {
            if (!Updater)
            {
                // Update the lobby once
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Past_MouseDown(object sender, MouseButtonEventArgs e)
        {
            KeyInput.Text = Clipboard.GetText();
        }

        private void Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Data.SettingCh.St.Key = KeyInput.Text;
            WriteSetting();
        }

        private void Join_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Data.HostOrClient = false;
            Data.KeyForJoin = KeyInput.Text;
            Data.JoinToServer = false;

            if (!Data.HostOrClient)
            {
                // Running client

                Client client = new Client();
                Crypt decrypt = new Crypt();
                Get getIpOrPort = new Get();


                // Lobby
                if (Data.JoinToServer)
                {

                    int tcpPort = 0;
                    Int32.TryParse(Data.webList[Data.LobbyNumber].Port, out tcpPort);
                    client.InitializeConnection(Data.webList[Data.LobbyNumber].IP, tcpPort);
                }
                // Local modul
                else
                {
                    string ipPort = decrypt.decry(Data.KeyForJoin);
                    int _tcpPort = 0;
                    Int32.TryParse(getIpOrPort.GetIpAndPort(ipPort, 1), out _tcpPort);
                    client.InitializeConnection(getIpOrPort.GetIpAndPort(ipPort, 0), _tcpPort);
                }


            }
        }

        private void Create_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Data.HostOrClient = true;
            Data.CreateToServer = false;

            WriteSetting();

            ComboBoxItem typeItem = (ComboBoxItem)LobbyType.SelectedItem;
            string value = typeItem.Content.ToString();

            if (Data.SettingCh.St.UseMod)
                Data.ModOrOrigin = "Mod";
            else
                Data.ModOrOrigin = "Orig";

            Data.LobbyTitle = Data.SettingCh.St.Name + "'s Lobby [ " + ModOrig.Text + " | " + typeItem.Content.ToString() + "]";

            Get _max = new Get();

            Data.MaxGamers = _max.GetMaxCountGamers(LobbyType.SelectedIndex);
            Data.GamersList.LobbyType = typeItem.Content.ToString();
            Data.GamersList.ModOrig = ModOrig.Text.ToString();

            Host host = new Host();
            host.StartListening(Data.SettingCh.St.TcpPort);
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWin Setting = new SettingsWin();
            Setting.ShowDialog();
        }

        private void View_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewWin View = new ViewWin();
            View.ShowDialog();
        }

        private void Sort1_Checked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = true;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;
            Sort2.IsChecked = false;
            Sort3.IsChecked = false;
            Sort4.IsChecked = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort2_Checked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = true;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;
            Sort1.IsChecked = false;
            Sort3.IsChecked = false;
            Sort4.IsChecked = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort3_Checked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = true;
            Data.SettingCh.St.Sort4 = false;
            Sort1.IsChecked = false;
            Sort2.IsChecked = false;
            Sort4.IsChecked = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort4_Checked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = true;
            Sort1.IsChecked = false;
            Sort2.IsChecked = false;
            Sort3.IsChecked = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort1_Unchecked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort2_Unchecked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort3_Unchecked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;

            // Update the lobby once
            if (!Updater)
            {
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void Sort4_Unchecked(object sender, RoutedEventArgs e)
        {
            Data.SettingCh.St.Sort1 = false;
            Data.SettingCh.St.Sort2 = false;
            Data.SettingCh.St.Sort3 = false;
            Data.SettingCh.St.Sort4 = false;

            // Update the lobby once
            if (!Updater)
            {

                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }

        private void WebDataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                SortMenu.IsOpen = true;
                SortMenu.HorizontalOffset = -285;
                SortMenu.VerticalOffset = -10;
                SortMenu.Visibility = Visibility.Visible;
            }
        }

        private void Gamesettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameSett GameSettings = new GameSett();
            GameSettings.ShowDialog();
        }

        private void ShowSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "settingsdown.gif";
            ImgSettings(path, LobbySettingGrid);

        }

        public void ImgSettings(string path, Grid someGrid)
        {
            if (File.Exists(path))
            {

                Image imagemap = new Image();
                ImageBrush myBrush = new ImageBrush();

                imagemap.Source = null; UpdateLayout(); GC.Collect();
                imagemap.Source = new BitmapImage(new Uri(path));
                myBrush.ImageSource = imagemap.Source;
                someGrid.Background = myBrush;
            }

        }

        private void ShowSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "settings.gif";
            ImgSettings(path, LobbySettingGrid);
        }

        private void ShowSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Data.SettingCh.St.ShowLobbySetting)
            {
                LobSettings.Visibility = Visibility.Collapsed;

                Data.SettingCh.St.ShowLobbySetting = false;
            }
            else
            {
                LobSettings.Visibility = Visibility.Visible;
                Data.SettingCh.St.ShowLobbySetting = true;
            }
        }

        private void Update_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Updater)
            {
                // Update the lobby once
                Web getListLobby = new Web();
                getListLobby.CreateLobbyListOnes();
                UpdateListThread();
            }
        }
        public static void UpdateListThread()

        {
            Thread sendThread = new Thread(new ThreadStart(Data.MainWin.UpdateList));
            sendThread.Start();
        }
        // Starting pinging
        public void UpdateList()
        {

            Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
            {
                string pathUpdate = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "update.gif";
                if (File.Exists(pathUpdate))
                {
                    var controller = ImageBehavior.GetAnimationController(UpdateImg);
                    controller.Play();

                }
                Refresh.IsEnabled = false;
            }));

            Updater = true;


            int per = 5;


            per = per * 1000;

            // Set the trigger interval
            _timerUpdate = new System.Timers.Timer(per);
            _timerUpdate.Elapsed += OnTimedEvent;
            _timerUpdate.AutoReset = true;
            _timerUpdate.Enabled = true;


            // Second timer controls the first every half second
            _timerUpdateStop = new System.Timers.Timer(200);
            _timerUpdateStop.Elapsed += OnTimedEventStop;
            _timerUpdateStop.AutoReset = true;
            _timerUpdateStop.Enabled = true;

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Updater = false;
        }

        private void OnTimedEventStop(Object source, ElapsedEventArgs e)
        {
            if (Updater == false)
            {
                _timerUpdateStop.Stop();
                _timerUpdateStop.Dispose();
                _timerUpdateStop.Enabled = false;
                _timerUpdate.Stop();
                _timerUpdate.Dispose();
                _timerUpdate.Enabled = false;

                Data.MainWin.Dispatcher.Invoke(new Action(delegate ()
                {
                    string pathUpdate = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "update.gif";
                    if (File.Exists(pathUpdate))
                    {
                        var controller = ImageBehavior.GetAnimationController(UpdateImg);
                        controller.Pause();
                        Refresh.IsEnabled = true;


                    }
                }));
            }
        }

        private void LobbyName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((LobbyName.Text.Length > 29) && (e.Key.ToString() != "Back") && (e.Key.ToString() != "Delete")
                    && (e.Key.ToString() != "Left") && (e.Key.ToString() != "Right"))
            {
                e.Handled = true;
            }
        }

        private void CommentInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((CommentInput.Text.Length > 199) && (e.Key.ToString() != "Back") && (e.Key.ToString() != "Delete")
                    && (e.Key.ToString() != "Left") && (e.Key.ToString() != "Right"))
            {
                e.Handled = true;
            }
        }

        private void PasswInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((PasswInput.Text.Length > 7) && (e.Key.ToString() != "Back") && (e.Key.ToString() != "Delete")
                 && (e.Key.ToString() != "Left") && (e.Key.ToString() != "Right"))
            {
                e.Handled = true;
            }
        }

        private void PasswInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            PassCount.Text = (8 - PasswInput.Text.Length).ToString(CultureInfo.InvariantCulture);
        }
    }
}

