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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Threading;
using WpfAnimatedGif;
using System.Timers;

using System.Net;
using System.Net.Sockets;




namespace TMC
{





    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LobbyWin : Window
    {

        Client sendToHost = new Client();

        // For car or map images 
        private static BitmapImage _image = new BitmapImage();
        private static Image _imagemap = new Image();
        private static ImageBrush _myBrush = new ImageBrush();
        private int count;

        // Recording Settings
        private void WriteSetting()
        {
            Data.SettingCh.St.Lobby_X_pos = this.Left;
            Data.SettingCh.St.Lobby_Y_pos = this.Top;
            Data.SettingCh.St.Lobby_Width = this.Width;
            Data.SettingCh.St.Lobby_Height = this.Height;
            if (WindowState == WindowState.Maximized)
            {
                Data.SettingCh.St.LobbyMaximized = true;
            }
            else
            {
                Data.SettingCh.St.LobbyMaximized = false;
            }

            Data.SettingCh.St.LobbyGridsp1 = LocalGrid.ColumnDefinitions[3].ActualWidth;
            if (Data.SettingCh.St.UseBottomhModule == false)
                Data.SettingCh.St.LobbyGridsp2 = LobbyMainGrid.RowDefinitions[0].ActualHeight;
            Data.SettingCh.St.LobbyGridsp3 = ChatListGrid.ColumnDefinitions[0].ActualWidth;
            Data.SettingCh.St.LobbyGridsp4 = ChatSplitGrid.RowDefinitions[0].ActualHeight;

            Data.SettingCh.WriteXml();
        }

        // Reading settings
        private void ReadSetting()
        {

            Data.SettingCh.ReadXml();

            this.Left = Data.SettingCh.St.Lobby_X_pos;
            this.Top = Data.SettingCh.St.Lobby_Y_pos;
            this.Width = Data.SettingCh.St.Lobby_Width;
            this.Height = Data.SettingCh.St.Lobby_Height;
            if (Data.SettingCh.St.LobbyMaximized)
            {
                WindowState = WindowState.Maximized;
            }

            LocalGrid.ColumnDefinitions[3].Width = new GridLength(Data.SettingCh.St.LobbyGridsp1, GridUnitType.Pixel);
            LobbyMainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.LobbyGridsp2, GridUnitType.Pixel);
            ChatListGrid.ColumnDefinitions[0].Width = new GridLength(Data.SettingCh.St.LobbyGridsp3, GridUnitType.Pixel);
            ChatSplitGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.LobbyGridsp4, GridUnitType.Pixel);

            if (Data.SettingCh.St.UseBouthModule == true)
            {
                LobbyMainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.LobbyGridsp2, GridUnitType.Pixel);
                LobbyMainGrid.RowDefinitions[1].Height = new GridLength(2, GridUnitType.Pixel);
                TopSplitter.IsEnabled = true;
                ListSettingGrid.RowDefinitions[1].Height = new GridLength(130, GridUnitType.Pixel);
            }
            else if (Data.SettingCh.St.UseTopModule == true)
            {
                LobbyMainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.LobbyGridsp2, GridUnitType.Pixel);
                LobbyMainGrid.RowDefinitions[1].Height = new GridLength(2, GridUnitType.Pixel);
                TopSplitter.IsEnabled = true;
                ListSettingGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
            }

            else if (Data.SettingCh.St.UseBottomhModule == true)
            {
                LocalGrid.IsEnabled = false;
                LocalGrid.Visibility = Visibility.Collapsed;
                LobbyMainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
                TopSplitter.IsEnabled = false;
                ListSettingGrid.RowDefinitions[1].Height = new GridLength(130, GridUnitType.Pixel);
            }


            ShowMap(Data.SettingCh.St.Map);
            ShowCar(Data.SettingCh.St.Car);

        }

        // Display card by number
        public void ShowMap(int num)
        {

            string path = Environment.CurrentDirectory + "\\data\\img\\maps\\" + "map_" + num + ".jpg";

            if (!Data.SettingCh.St.UseBottomhModule)
            {
                if (File.Exists(path))
                {

                    _myBrush = new ImageBrush();
                    _imagemap.Source = new BitmapImage(new Uri(path));
                    _myBrush.ImageSource = _imagemap.Source;
                    Grid grid = new Grid();
                    MapSettingGrid.Background = _myBrush;
                    Get getMap = new Get();
                    LineMap.Text = getMap.GetMapToIndex(Data.SettingCh.St.Map);
                }
            }
        }

        // Change card number
        public void ContextMap(int index)
        {

            Get getMap = new Get();
            Data.SettingCh.St.Map = index;

            ShowMap(Data.SettingCh.St.Map);

            if (Data.HostOrClient)
            {
                Data.GamersList.Map = Data.SettingCh.St.Map;
                GamerList gamerListMap = new GamerList();
                gamerListMap.Type = 1;
                gamerListMap.Map = Data.SettingCh.St.Map;
                gamerListMap.Score = Data.SettingCh.St.Score;

                Host.SendAllThread(gamerListMap);
            }
        }

        public void ShowCar(int num)
        {
            if (!Data.SettingCh.St.UseBottomhModule)
            {
                string path = Environment.CurrentDirectory + "\\data\\img\\cars\\" + "car_" + num + ".gif";
                if (File.Exists(path))
                {
                    _image = null;
                    ImageBehavior.SetAnimatedSource(CarSettingImg, null); UpdateLayout(); GC.Collect();
                    _image = new BitmapImage();
                    _image.BeginInit();
                    _image.UriSource = new Uri(path);
                    _image.EndInit();

                    ImageBehavior.SetAnimatedSource(CarSettingImg, _image);
                    _image.Freeze();

                }

            }

        }

        public void ContextCar(int index)
        {
            Data.SettingCh.St.Car = index;
            ShowCar(Data.SettingCh.St.Car);
            Get getCar = new Get();
            LineCar.Text = getCar.GetCarToIndex(Data.SettingCh.St.Car);


            if (Data.HostOrClient)
            {
                Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                GamerList gamerListCar = new GamerList();
                gamerListCar.Type = 2;
                gamerListCar.Number = 0;
                gamerListCar.Map = Data.SettingCh.St.Car;

                Host.SendAllThread(gamerListCar);
            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Car = Data.SettingCh.St.Car;

                sendToHost.SendMessagesThread(1, Gamer);
            }

        }

        public void ClientLobby()
        {
            GamerGrid.Columns[5].Visibility = Visibility.Collapsed;
            BtnClose.Visibility = Visibility.Hidden;
            BtnGo.Visibility = Visibility.Hidden;
            LineScor.Visibility = Visibility.Hidden;
            LineScoreBord.Visibility = Visibility.Hidden;


            ClmBtnClose.Width = new GridLength(0, GridUnitType.Pixel);
            ClmBtnGo.Width = new GridLength(0, GridUnitType.Pixel);
            ClmBtnUpd.Width = new GridLength(0, GridUnitType.Pixel);
            LineBtnGo.Visibility = Visibility.Hidden;
            BtnUpdBottom.Visibility = Visibility.Hidden;
            BtnCloseBottom.Visibility = Visibility.Hidden;
            LineClose.Width = new GridLength(0, GridUnitType.Pixel);
            LineUdp.Width = new GridLength(0, GridUnitType.Pixel);
            LineGo.Width = new GridLength(0, GridUnitType.Pixel);
        }

        public void ScoreChange(int score)
        {
            ScoreClient.Text = score.ToString();
            LineScoreClient.Text = score.ToString();
        }

        public void ClearImg()
        {

            MapSettingGrid = null;
            if (_image != null)
            {
                ImageBehavior.SetAnimatedSource(CarSettingImg, null);
            }
            if (_myBrush != null)
            {
                _myBrush.ImageSource = null;
            }
            if (_imagemap != null)
            {
                _imagemap.Source = null;
            }

            Data.LobbyLink.UpdateLayout();
            GC.Collect();
        }


        public void LockLobbyLd()
        {
            LocalGrid.IsEnabled = false;
            LineSettingGrid.IsEnabled = false;
        }

        public void LobbyClose()
        {
            this.Close();
        }

        public void CheckPing()
        {
            Get readyPing = new Get();
            if (readyPing.GetReadyPing())
            {
                Ready.IsEnabled = true;
            }
            else
            {
                Ready.IsEnabled = false;
            }
        }

        public void FlashWin()
        {
            Focus();
            Activate();
        }

        private static System.Timers.Timer TimerUptate;
        private static System.Timers.Timer TimerUptateStop;

        // Launch form
        public LobbyWin()
        {
            InitializeComponent();
            ReadSetting();

            Data.ChatStatic = Chat;
            Data.BtnReady = Ready;
            Data.LineBtnReady = LineBtnReady;
            Data.BtnLineGo = LineBtnGo;
            Data.Go = BtnGo;
            Data.GoLine = LineBtnGo;

            Get getCar = new Get();
            LineCar.Text = getCar.GetCarToIndex(Data.SettingCh.St.Car);
            GamerGrid.ItemsSource = Data.gamerList;
            Get getTeam = new Get();
            var bc = new BrushConverter();
            TeamBorder.Background = (Brush)bc.ConvertFrom(getTeam.GetColorTeam(Data.SettingCh.St.Team));
            LineTeamBorder.Background = (Brush)bc.ConvertFrom(getTeam.GetColorTeam(Data.SettingCh.St.Team));

            // Add list instance
            List<Gamers> gamer = new List<Gamers>();
            Data.GamersList.gamer = gamer;

            string _pathScore = Environment.CurrentDirectory + "\\data\\img\\contr\\score.gif";
            if (File.Exists(_pathScore))
            {
                Image ImageContainer = new Image();
                ImageSource image = new BitmapImage(new Uri(_pathScore, UriKind.Absolute));
                ImgScore.Source = image;

            }
            else
            {
                ScoreText.Text = "Score";
            }
            string _pathTeam = Environment.CurrentDirectory + "\\data\\img\\contr\\team.gif";
            if (File.Exists(_pathTeam))
            {
                Image ImageContainer = new Image();
                ImageSource image = new BitmapImage(new Uri(_pathTeam, UriKind.Absolute));
                imgTeam.Source = image;

            }
            else
            {
                TeamText.Text = "Team";
            }

            if (!Data.HostOrClient)
            {
                // Running client
                ClientLobby();
            }
            else
            {
                BtnGo.IsEnabled = false;
                LineScoreBorder.Visibility = Visibility.Hidden;
                ScoreBorder.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Team.Text = Data.SettingCh.St.Team.ToString();
            LineTeam.Text = Data.SettingCh.St.Team.ToString();
            TextSend.Document.Blocks.Clear();
            Chat.Document.Blocks.Clear();

            // Running host
            if (Data.HostOrClient)
            {
                Ready.IsEnabled = true;

                Score.Text = Data.SettingCh.St.Score.ToString();

                if ((Data.GamersList.LobbyType != "FFA" && Data.SettingCh.St.Team > 2) || Data.SettingCh.St.Team < 0)
                    Data.SettingCh.St.Team = 2;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {

            // If we close, then release the lobby key
            Data.LobbyOpen = true;

            WriteSetting();

            if (Data.HostOrClient)
            {

                if (Data.CreateToServer)
                {
                    // Start the removal of the lobby in a new thread

                    Data.FollowToLobby = false;
                    Web delateLobby = new Web();
                    delateLobby.DeleteLobbyThread();
                }

                Host close = new Host();

                close.StopListening();
                close.CloseC();

                Data.gamerList.Clear();
                Data.GamersList.gamer.Clear();

                Data.UdpWorkSend = false;
                Data.UdpWorkRec = false;
                if (Udp.receiver_server != null)
                {
                    Udp.receiver_server.Close();
                }

            }
            else
            {

                Gamers Gamer = new Gamers();

                Gamer.Team = 2;

                sendToHost.SendMessagesThread(7, Gamer);

            }

            // If the module with pictures is enabled
            if (!Data.SettingCh.St.UseBottomhModule)
            {
                if (_myBrush != null)
                {
                    // Clear image resources
                    ImageBehavior.SetAnimatedSource(CarSettingImg, null); UpdateLayout(); GC.Collect();
                    _myBrush.ImageSource = null; _imagemap.Source = null; _myBrush = null; UpdateLayout(); GC.Collect();
                }

            }

            Data.MainWin.UnLock();
        }

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(MapSettingGrid);

            // Host running
            if (Data.HostOrClient)
            {

                if (e.ChangedButton == MouseButton.Right)
                {
                    Sound.PlaySound("carmap.mp3");

                    MapMenu.IsOpen = false;
                    MapMenu.Visibility = Visibility.Collapsed;

                    if (Data.SettingCh.St.Map != 10)
                        Data.SettingCh.St.Map++;
                    else
                        Data.SettingCh.St.Map = 0;
                }
                else if (e.ChangedButton == MouseButton.Left)
                {
                    Sound.PlaySound("carmap.mp3");

                    if (Data.SettingCh.St.Map != 0)
                        Data.SettingCh.St.Map--;
                    else
                        Data.SettingCh.St.Map = 10;

                }
                else if (e.ChangedButton == MouseButton.Middle)
                {
                    Sound.PlaySound("carmap.mp3");

                    MapMenu.IsOpen = true;

                    MapMenu.HorizontalOffset = -140;

                    MapMenu.Visibility = Visibility.Visible;
                    return;
                }

                Get getMap = new Get();

                ShowMap(Data.SettingCh.St.Map);

                if (Data.HostOrClient)
                {
                    Data.GamersList.Map = Data.SettingCh.St.Map;
                    GamerList gamerListMap = new GamerList();
                    gamerListMap.Type = 1;
                    gamerListMap.Map = Data.SettingCh.St.Map;

                    Host.SendAllThread(gamerListMap);
                }

            }
        }

        private void Car_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Right)
            {

                if (Data.SettingCh.St.Car != 13)
                {
                    Data.SettingCh.St.Car++;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {
                    Data.SettingCh.St.Car = 0;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

            }
            else
            {
                if (Data.SettingCh.St.Car != 0)
                {
                    Data.SettingCh.St.Car--;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {
                    Data.SettingCh.St.Car = 13;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }


            }

            ShowCar(Data.SettingCh.St.Car);
        }

        private void Score_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Score.ActualHeight > 0)
                Score.FontSize = Score.ActualHeight / (1.6);

        }

        private void Score_TextChanged(object sender, TextChangedEventArgs e)
        {

            int _score = 0;
            Int32.TryParse(Score.Text.ToString(), out _score);
            Data.SettingCh.St.Score = _score;
            Data.GamersList.Score = _score;

            if (Data.HostOrClient)
            {
                GamerList gamerListMap = new GamerList();
                gamerListMap.Type = 6;
                gamerListMap.Score = _score;
                gamerListMap.Map = Data.SettingCh.St.Map;

                Host.SendAllThread(gamerListMap);
            }

            LineScore.Text = Score.Text;

        }

        private void Ready_Click(object sender, RoutedEventArgs e)
        {


            if (Data.HostOrClient)
            {
                Data.GamersList.Type = 3;

                // Host
                if (Data.GamersList.gamer.Count > 1)
                {

                    if ((string)Ready.Content == "Ready")
                    {
                        for (int i = 1; i < Data.GamersList.gamer.Count; i++)
                        {
                            // If the player is on the list clicked ready
                            if (Data.GamersList.gamer[i].Status != 0)
                            {
                                // Compare his car and ours
                                if (Data.GamersList.gamer[0].Car == Data.GamersList.gamer[i].Car)
                                {
                                    Set setMassage = new Set();
                                    setMassage.SetMassageCar(i);
                                    return;
                                }

                            }

                        }

                        // Then I change my status to ready
                        Data.GamersList.gamer[0].GStatus = 1;

                        Data.LobbyLink.Lock();

                        GamerList gamerListTeam = new GamerList();
                        gamerListTeam.Type = 3;
                        gamerListTeam.Number = 0;
                        gamerListTeam.Map = 1;

                        Host.SendAllThread(gamerListTeam);
                    }
                    else
                    {
                        Data.GamersList.gamer[0].GStatus = 0;

                        Data.LobbyLink.UnLock();

                        GamerList gamerListTeam = new GamerList();
                        gamerListTeam.Type = 3;
                        gamerListTeam.Number = 0;
                        gamerListTeam.Map = 0;

                        Host.SendAllThread(gamerListTeam);

                    }

                }
                // If one in the room I choose when I want and what I want
                else
                {
                    if ((string)Ready.Content == "Ready")
                    {
                        Data.LobbyLink.Lock();
                        Data.GamersList.gamer[0].Status = 1;
                        Data.GamersList.gamer[0].GStatus = 1;

                    }
                    else
                    {
                        Data.GamersList.gamer[0].Status = 0;
                        Data.GamersList.gamer[0].GStatus = 0;
                        Data.LobbyLink.UnLock();

                    }

                }

                var bc = new BrushConverter();
                Get ready = new Get();
                if (ready.GetReady())
                {
                    Data.Go.IsEnabled = true;
                    Data.GoLine.IsEnabled = true;
                    Data.Go.Background = (Brush)bc.ConvertFrom("#11dc11");
                    Data.GoLine.Background = (Brush)bc.ConvertFrom("#11dc11");
                    if (Data.Go != null)
                        Data.Go.Focus();
                    else
                        Data.GoLine.Focus();
                }
                else
                {
                    Data.Go.Background = (Brush)bc.ConvertFrom("#d4d0c8");
                    Data.Go.IsEnabled = false;
                    Data.GoLine.Background = (Brush)bc.ConvertFrom("#d4d0c8");
                    Data.GoLine.IsEnabled = false;
                }

            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Car = Data.SettingCh.St.Car;
                if ((string)Ready.Content == "Ready")
                {

                    Gamer.Status = 1;

                    sendToHost.SendMessages(3, Gamer);

                }
                else
                {
                    Gamer.Status = 0;
                    sendToHost.SendMessagesThread(3, Gamer);

                }

            }

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGridSplitterHeights();
        }

        private void ResizeGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGridSplitterHeights();
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UpdateGridSplitterHeights();
        }

        private void UpdateGridSplitterHeights()
        {
            C0.MaxHeight = Math.Min(ChatSplitGrid.ActualHeight, ChatSplitGrid.MaxHeight) - (C2.MinHeight + 5);
            if (Data.SettingCh.St.UseBottomhModule != true)
            {

                if ((Math.Min(LobbyMainGrid.ActualHeight, LobbyMainGrid.MaxHeight) - (C5.MinHeight + 5)) > 0)
                    C4.MaxHeight = Math.Min(LobbyMainGrid.ActualHeight, LobbyMainGrid.MaxHeight) - (C5.MinHeight + 5);
                else
                    C4.MaxHeight = 1;
            }
            else
            {
                C4.MaxHeight = 0;

            }

            C6.MaxWidth = Math.Min(LobbyMainGrid.ActualWidth, LobbyMainGrid.MaxWidth) - (C7.MinWidth + 5);
            if (Data.SettingCh.St.UseBottomhModule != true)
            {
                C8.MaxWidth = Math.Min(LocalGrid.ActualWidth, LocalGrid.MaxWidth) - (C9.MinWidth + 5);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (Data.HostOrClient)
            {

                string _pathScore = Environment.CurrentDirectory + "\\data\\img\\contr\\scoredown.gif";
                if (File.Exists(_pathScore))
                {
                    Image ImageContainer = new Image();
                    ImageSource image = new BitmapImage(new Uri(_pathScore, UriKind.Absolute));
                    ImgScore.Source = image;

                }


                if (e.ChangedButton == MouseButton.Right)
                {
                    Sound.PlaySound("score.mp3");

                    Data.SettingCh.St.Score += 10;
                    Score.Text = Data.SettingCh.St.Score.ToString();

                }
                else
                {
                    Sound.PlaySound("score.mp3");

                    if (Data.SettingCh.St.Score >= 10)
                        Data.SettingCh.St.Score -= 10;
                    else if (Data.SettingCh.St.Score > 0)
                        Data.SettingCh.St.Score--;

                    Score.Text = Data.SettingCh.St.Score.ToString();
                }
            }
        }

        private void Team_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Team.ActualHeight > 0 && Score.ActualHeight != 0)
                Team.FontSize = Score.ActualHeight / (1.6);
        }


        private void CarSettingImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(CarSettingImgGrid);


            if (e.ChangedButton == MouseButton.Right)
            {
                Sound.PlaySound("carmap.mp3");

                if (Data.SettingCh.St.Car != 12)
                {
                    Data.SettingCh.St.Car++;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {
                    Data.SettingCh.St.Car = 0;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }



            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                Sound.PlaySound("carmap.mp3");

                if (Data.SettingCh.St.Car != 0)
                {
                    Data.SettingCh.St.Car--;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {

                    Data.SettingCh.St.Car = 12;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }


            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                Sound.PlaySound("carmap.mp3");

                CarMenu.IsOpen = true;
                CarMenu.HorizontalOffset = -140;
                CarMenu.Visibility = Visibility.Visible;
                return;
            }

            ShowCar(Data.SettingCh.St.Car);
            Get _getCar = new Get();
            LineCar.Text = _getCar.GetCarToIndex(Data.SettingCh.St.Car);


            if (Data.HostOrClient)
            {

                GamerList gamerListCar = new GamerList();
                gamerListCar.Type = 2;
                gamerListCar.Number = 0;
                gamerListCar.Map = Data.SettingCh.St.Car;

                Host.SendAllThread(gamerListCar);

            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Car = Data.SettingCh.St.Car;

                sendToHost.SendMessagesThread(1, Gamer);
            }

            CarMenu.IsOpen = false;
            CarMenu.Visibility = Visibility.Collapsed;
            e.Handled = true;

        }

        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {

            Get getColor = new Get();
            string getText = new TextRange(TextSend.Document.ContentStart, TextSend.Document.ContentEnd).Text;
            Set setMassage = new Set();

            if (getText.Length > 500)
            {
                getText = getText.Substring(0, 500);
            }

            if (getText != "" && getText != " ")
            {

                Sound.PlaySound("send.mp3");

                if (Data.HostOrClient)
                {


                    setMassage.SetMassage(0, getText.Trim());

                    Data.GamersList.Type = 4;
                    Data.GamersList.Message = getText.Trim();

                    Host.SendAllThread(Data.GamersList);

                    TextSend.Document.Blocks.Clear();


                }
                else
                {
                    Gamers Gamer = new Gamers();
                    Gamer.Number = 0;
                    Gamer.Message = getText.Trim();

                    sendToHost.SendMessagesThread(4, Gamer);
                    TextSend.Document.Blocks.Clear();
                }

            }

        }

        private void TeamBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (Data.GamersList.LobbyType == "FFA")
                {
                    if (Data.SettingCh.St.Team <= 7)
                    {
                        Data.SettingCh.St.Team++;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 0;
                    }

                }
                else
                {

                    if (Data.SettingCh.St.Team < 2)
                    {
                        Data.SettingCh.St.Team++;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 1;
                    }

                }


            }
            else
            {
                if (Data.GamersList.LobbyType == "FFA")
                {
                    if (Data.SettingCh.St.Team > 0)
                    {
                        Data.SettingCh.St.Team--;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 8;
                    }
                }
                else
                {
                    if (Data.SettingCh.St.Team > 1)
                    {
                        Data.SettingCh.St.Team--;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 2;
                    }
                }


            }
            Team.Text = Data.SettingCh.St.Team.ToString();
            Data.GamersList.Type = 2;
            Data.GamersList.gamer[0].Team = Data.SettingCh.St.Team;
            Data.GamersList.gamer[0].GTeam = Data.SettingCh.St.Team;

            Get _getTeam = new Get();
            var bc = new BrushConverter();
            TeamBorder.Background = (Brush)bc.ConvertFrom(_getTeam.GetColorTeam(Data.SettingCh.St.Team));
            TeamBorderImg.Background = (Brush)bc.ConvertFrom(_getTeam.GetColorTeam(Data.SettingCh.St.Team));
            LineTeamBorder.Background = (Brush)bc.ConvertFrom(_getTeam.GetColorTeam(Data.SettingCh.St.Team));

            Host.SendAllThread(Data.GamersList);
        }

        private void LineScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            Score.Text = LineScore.Text;
        }

        private void TeamBorderImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Sound.PlaySound("score.mp3");

            string _pathTeam = Environment.CurrentDirectory + "\\data\\img\\contr\\teamdown.gif";
            if (File.Exists(_pathTeam))
            {
                Image ImageContainer = new Image();
                ImageSource image = new BitmapImage(new Uri(_pathTeam, UriKind.Absolute));
                imgTeam.Source = image;
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                if (Data.GamersList.LobbyType == "FFA")
                {
                    if (Data.SettingCh.St.Team <= 7)
                    {
                        Data.SettingCh.St.Team++;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 0;
                    }

                }
                else
                {

                    if (Data.SettingCh.St.Team < 2)
                    {
                        Data.SettingCh.St.Team++;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 1;
                    }

                }


            }
            else
            {
                if (Data.GamersList.LobbyType == "FFA")
                {
                    if (Data.SettingCh.St.Team > 0)
                    {
                        Data.SettingCh.St.Team--;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 8;
                    }
                }
                else
                {
                    if (Data.SettingCh.St.Team > 1)
                    {
                        Data.SettingCh.St.Team--;
                    }
                    else
                    {
                        Data.SettingCh.St.Team = 2;
                    }
                }


            }

            Team.Text = Data.SettingCh.St.Team.ToString();

            LineTeam.Text = Data.SettingCh.St.Team.ToString();
            Get _getTeam = new Get();
            var bc = new BrushConverter();
            TeamBorder.Background = (Brush)bc.ConvertFrom(_getTeam.GetColorTeam(Data.SettingCh.St.Team));
            LineTeamBorder.Background = (Brush)bc.ConvertFrom(_getTeam.GetColorTeam(Data.SettingCh.St.Team));

            if (Data.HostOrClient)
            {
                Data.GamersList.gamer[0].GTeam = Data.SettingCh.St.Team;

                GamerList gamerListTeam = new GamerList();
                gamerListTeam.Type = 11;
                gamerListTeam.Number = 0;
                gamerListTeam.Map = Data.SettingCh.St.Team;

                Host.SendAllThread(gamerListTeam);

            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Team = Data.SettingCh.St.Team;

                sendToHost.SendMessagesThread(2, Gamer);
            }

        }

        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            Ld startLd = new Ld();

            Sound.PlaySound("go.mp3");

            startLd.StartLDThread();

        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {

            Web update = new Web();
            update.UpdLobbyThread();

            UpdateInfLobbyThread();

        }

        public static void UpdateInfLobbyThread()
        {
            Thread sendThread = new Thread(new ThreadStart(Data.LobbyLink.UpdateList));
            sendThread.Start();
        }

        // Start pinging
        public void UpdateList()
        {
            int period = 5;

            BtnUpdate.Dispatcher.Invoke(new Action(delegate ()
            {
                BtnUpdate.IsEnabled = false;
                UpdateTxt.Text = "Unlock[5]";
                BtnUpdBottom.IsEnabled = false;
                BtnUpdBottom.Content = "Unlock[5]";
                count = 5;
            }));

            period = period * 1000;

            // Set the response interval
            TimerUptate = new System.Timers.Timer(period);
            TimerUptate.Elapsed += OnTimedEvent;
            TimerUptate.AutoReset = true;
            TimerUptate.Enabled = true;

            // Second timer controls the first every half second
            TimerUptateStop = new System.Timers.Timer(1000);
            TimerUptateStop.Elapsed += OnTimedEventStop;
            TimerUptateStop.AutoReset = true;
            TimerUptateStop.Enabled = true;

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            BtnUpdate.Dispatcher.Invoke(new Action(delegate ()
            {
                BtnUpdate.IsEnabled = true;
                BtnUpdBottom.Content = "Upd";
                BtnUpdBottom.IsEnabled = true;
                UpdateTxt.Text = "Upd";
                TimerUptateStop.Stop();
                TimerUptateStop.Dispose();
                TimerUptateStop.Enabled = false;
                TimerUptate.Stop();
                TimerUptate.Dispose();
                TimerUptate.Enabled = false;

            }));

        }

        private void OnTimedEventStop(Object source, ElapsedEventArgs e)
        {
            UpdateTxt.Dispatcher.Invoke(new Action(delegate ()
            {
                count--;
                UpdateTxt.Text = "Unlock[" + count + "]";
                BtnUpdBottom.Content = "Unlock[" + count + "]";

            }));

        }



        private void GamerGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Gamers path = GamerGrid.SelectedItem as Gamers;
            int parse = GamerGrid.SelectedIndex;
            if (Data.HostOrClient)
            {


                if (parse > 0)
                {

                    if (GamerGrid.CurrentCell.Column.DisplayIndex == 1)
                    {
                        Data.LastNumber = parse;

                        GamerList gamerListMap = new GamerList();
                        gamerListMap.Type = 13;
                        Host.SendToNumberThread(parse, gamerListMap);
                    }

                }
                else if (parse == 0)
                {
                    if (GamerGrid.CurrentCell.Column.DisplayIndex == 1)
                    {
                        CheckPortsThread();
                    }

                }

            }


            if (parse != Data.MyNamber && parse > -1)
            {
                if (GamerGrid.CurrentCell.Column.DisplayIndex == 2)
                {
                    TextSend.Focus();
                    TextSend.Document.Blocks.Clear();
                    TextSend.Document.Blocks.Add(new Paragraph(new Run(Data.GamersList.gamer[parse].Name + ", ")));
                    TextPointer richTextBox = TextSend.CaretPosition;
                    richTextBox = richTextBox.DocumentEnd;
                    TextSend.CaretPosition = richTextBox;
                }

            }
        }

        private void CheckPortsThread()

        {
            Thread sendThread = new Thread(new ThreadStart(CheckPorts));
            sendThread.Start();
        }

        private void CheckPorts()
        {
            TcpClient TcpServer;
            UdpClient send;
            string ip = "";
            Set message = new Set();
            message.SetText("Test ports:");
            try
            {
                Random randomTcpPort = new Random();
                int myTcpPort;
                myTcpPort = randomTcpPort.Next(49152, 65535);
                IPEndPoint localIP = new IPEndPoint(IPAddress.Any, myTcpPort);
                TcpServer = new TcpClient(localIP);
                Get getIp = new Get();
                ip = getIp.GetPublicIP();
                int port = 0;
                Int32.TryParse(Data.SettingCh.St.TcpPort, out port);
                TcpServer.Connect(ip, port); //ip

                NetworkStream tcpStream = TcpServer.GetStream();

                Gamers Gamer = new Gamers();
                Gamer.Type = 0;
                Gamer.Name = Data.SettingCh.St.Name;
                Gamer.Status = 0;
                Gamer.Message = "tcptest";
                tcpStream.Write(Json.WriteClient(Gamer), 0, Json.WriteClient(Gamer).Length);
                tcpStream.Flush();

                TcpServer.Close();

            }
            catch
            {

            }

            try
            {
                Random randomUdpPort = new Random();
                int myUdppPort;
                myUdppPort = randomUdpPort.Next(49152, 65535);
                send = new UdpClient(myUdppPort);
                int port = 0;
                Int32.TryParse(Data.SettingCh.St.UdpPort, out port);

                send.Connect(ip, port); //ip

                string messag = "u";
                byte[] data = Encoding.Unicode.GetBytes(messag);

                send.Send(data, data.Length, Data.GamersList.gamer[0].Udp);
                send.Close();

            }
            catch
            {

            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlockClose.Text == "Close?")
            {
                LockLobby();
            }
            else
            {
                UnLockLobby();
            }
        }

        public void LockLobby()
        {
            Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.LobbyOpen = false;
                this.Title = Data.LobbyTitle + "  Closed";
                TextBlockClose.Text = "Open?";
                BtnCloseBottom.Content = "Open Room?";

            }));
        }

        public void UnLockLobby()
        {
            Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.LobbyOpen = true;
                this.Title = Data.LobbyTitle;
                TextBlockClose.Text = "Close?";
                BtnCloseBottom.Content = "Close Room?";

            }));
        }

        private void BtnGetKey_Click(object sender, RoutedEventArgs e)
        {

            if (Data.HostOrClient)
            {

                Set printData = new Set();

                Crypt crypt = new Crypt();

                Clipboard.Clear();

                if (Data.CreateToServer)
                {
                    Get getIp = new Get();
                    string ip = getIp.GetPublicIP();
                    Clipboard.SetText(crypt.cry(ip + ":" + Data.SettingCh.St.TcpPort));

                }
                else
                {
                    if (Data.SettingCh.St.IP != null && Data.SettingCh.St.IP != "")
                    {
                        Clipboard.SetText(crypt.cry(Data.SettingCh.St.IP + ":" + Data.SettingCh.St.TcpPort));
                    }

                    else
                    {
                        Get getIp = new Get();
                        string ip = getIp.GetPublicIP();
                        Clipboard.SetText(crypt.cry(ip + ":" + Data.SettingCh.St.TcpPort));
                    }

                }

            }

            else
            {
                Clipboard.Clear();
                Clipboard.SetText(Data.KeyForJoin);
            }
        }

        private void BtnGetKeyBottom_Click(object sender, RoutedEventArgs e)
        {
            if (Data.HostOrClient)
            {
                Set _printData = new Set();

                Crypt _crypt = new Crypt();

                Clipboard.Clear();
                Clipboard.SetText(_crypt.cry(Data.SettingCh.St.IP + ":" + Data.SettingCh.St.TcpPort));
            }

            else
            {
                Clipboard.Clear();
                Clipboard.SetText(Data.KeyForJoin);
            }
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            Host.RemoveUserNumber(GamerGrid.SelectedIndex);
        }

        private void map0(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(0);
        }

        private void map1(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(1);
        }

        private void map2(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(2);
        }

        private void map3(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(3);
        }

        private void map4(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(4);
        }

        private void map5(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(5);
        }

        private void map6(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(6);
        }

        private void map7(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(7);
        }

        private void map8(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(8);
        }

        private void map9(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(9);
        }

        private void map10(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextMap(10);
        }

        private void car0(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(0);
        }

        private void car1(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(1);
        }

        private void car2(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(2);
        }

        private void car3(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(3);
        }
        private void car4(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(4);
        }
        private void car5(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(5);
        }
        private void car6(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(6);
        }
        private void car7(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(7);
        }

        private void car8(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(8);
        }
        private void car9(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(9);
        }
        private void car10(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(10);
        }
        private void car11(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(11);
        }
        private void car12(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(12);
        }
        private void car13(object sender, RoutedEventArgs e)
        {
            Sound.PlaySound("carmap.mp3");
            ContextCar(13);
        }
        private void SendingMessage()
        {
            Get getColor = new Get();
            string getText = new TextRange(TextSend.Document.ContentStart, TextSend.Document.ContentEnd).Text;
            Set setMassage = new Set();

            if (getText.Length > 500)
            {
                getText = getText.Substring(0, 500);
            }

            if (getText != "" && getText != " ")
            {
                Sound.PlaySound("send.mp3");

                if (Data.HostOrClient)
                {
                    setMassage.SetMassage(0, getText.Trim());

                    Data.GamersList.Type = 4;
                    Data.GamersList.Message = getText.Trim();

                    Host.SendAllThread(Data.GamersList);

                    TextSend.Document.Blocks.Clear();
                }
                else
                {
                    Gamers Gamer = new Gamers();
                    Gamer.Number = 0;
                    Gamer.Message = getText.Trim();

                    sendToHost.SendMessagesThread(4, Gamer);
                    TextSend.Document.Blocks.Clear();
                }
            }
        }

        public void CloseLobby()
        {
            this.Close();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (Data.HostOrClient)
            {

                string _pathScore = Environment.CurrentDirectory + "\\data\\img\\contr\\score.gif";
                if (File.Exists(_pathScore))
                {
                    Image ImageContainer = new Image();
                    ImageSource image = new BitmapImage(new Uri(_pathScore, UriKind.Absolute));
                    ImgScore.Source = image;

                }
            }
        }

        private void BtnCloseBottom_Click(object sender, RoutedEventArgs e)
        {

            if (BtnCloseBottom.Content.ToString() == "Close Room?")
            {
                Data.LobbyOpen = false;
                this.Title = Data.LobbyTitle + "  Closed";
                BtnCloseBottom.Content = "Open Room?";
                TextBlockClose.Text = "Open?";

            }
            else
            {
                Data.LobbyOpen = true;
                this.Title = Data.LobbyTitle;
                BtnCloseBottom.Content = "Close Room?";
                TextBlockClose.Text = "Close?";
            }
        }

        private void BtnUpdBottom_Click(object sender, RoutedEventArgs e)
        {

            Web update = new Web();
            update.UpdLobbyThread();

            UpdateInfLobbyThread();
        }

        private void LineBtnGo_Click(object sender, RoutedEventArgs e)
        {
            Ld startLd = new Ld();

            Sound.PlaySound("go.mp3");

            startLd.StartLDThread();
        }

        private void Score_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (Score.ActualHeight != 0)
                Score.FontSize = Score.ActualHeight / (1.6);
        }

        private void ScoreClient_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScoreClient.ActualHeight != 0)
                ScoreClient.FontSize = Score.ActualHeight / (1.6);
        }

        private void TeamBorderImg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string _pathTeam = Environment.CurrentDirectory + "\\data\\img\\contr\\team.gif";
            if (File.Exists(_pathTeam))
            {
                Image ImageContainer = new Image();
                ImageSource image = new BitmapImage(new Uri(_pathTeam, UriKind.Absolute));
                imgTeam.Source = image;

            }
        }


        public void Lock()
        {

            Sound.PlaySound("go.mp3");

            Ready.Content = "Cancel";

            MapSettingGrid.IsEnabled = false;
            CarSettingImgGrid.IsEnabled = false;
            SetGrid1.IsEnabled = false;
            SetGrid2.IsEnabled = false;
            SetGrid3.IsEnabled = false;
            CarSettingImg.IsEnabled = false;
            if (_image != null)
            {
                var controller = ImageBehavior.GetAnimationController(CarSettingImg);
                controller.Pause();
            }

            LineBtnReady.Content = "Cancel";
            LineSetGridMap.IsEnabled = false;
            LineGridCarTeam.IsEnabled = false;
            LineSetGridClose.IsEnabled = false;
        }

        public void UnLock()
        {
            Sound.StopSound("go.mp3");

            Ready.Content = "Ready";

            MapSettingGrid.IsEnabled = true;
            CarSettingImgGrid.IsEnabled = true;
            SetGrid1.IsEnabled = true;
            SetGrid2.IsEnabled = true;
            SetGrid3.IsEnabled = true;
            CarSettingImg.IsEnabled = true;

            var controller = ImageBehavior.GetAnimationController(CarSettingImg);
            controller.Play();

            LineBtnReady.Content = "Ready";
            LineSetGridMap.IsEnabled = true;
            LineGridCarTeam.IsEnabled = true;
            LineSetGridClose.IsEnabled = true;
        }

        public void Go()
        {

            var bc = new BrushConverter();
            Get ready = new Get();
            if (ready.GetReady())
            {
                BtnGo.IsEnabled = true;
                BtnGo.Background = (Brush)bc.ConvertFrom("#11dc11");
                BtnGo.Focus();
                LineBtnGo.IsEnabled = true;
                LineBtnGo.Background = (Brush)bc.ConvertFrom("#11dc11");
            }
            else
            {
                BtnGo.Background = (Brush)bc.ConvertFrom("#d4d0c8");
                BtnGo.IsEnabled = false;
                LineBtnGo.Background = (Brush)bc.ConvertFrom("#d4d0c8");
                LineBtnGo.IsEnabled = false;
            }
        }

        private void CarSettingImgGrid_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Right || e.Key == Key.Up)
            {
                Sound.PlaySound("carmap.mp3");

                CarMenu.IsOpen = false;
                CarMenu.Visibility = Visibility.Collapsed;

                if (Data.SettingCh.St.Car != 12)
                {
                    Data.SettingCh.St.Car++;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {
                    Data.SettingCh.St.Car = 0;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

            }
            else if (e.Key == Key.Left || e.Key == Key.Down)
            {
                Sound.PlaySound("carmap.mp3");

                if (Data.SettingCh.St.Car != 0)
                {
                    Data.SettingCh.St.Car--;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {

                    Data.SettingCh.St.Car = 12;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }


            }
            else if (e.Key == Key.Return)
            {
                Sound.PlaySound("carmap.mp3");

                CarMenu.IsOpen = true;
                CarMenu.HorizontalOffset = -140;
                CarMenu.Visibility = Visibility.Visible;

                Keyboard.Focus(CarMenu);
                return;
            }

            ShowCar(Data.SettingCh.St.Car);
            Get getCar = new Get();
            LineCar.Text = getCar.GetCarToIndex(Data.SettingCh.St.Car);


            if (Data.HostOrClient)
            {

                GamerList gamerListCar = new GamerList();
                gamerListCar.Type = 2;
                gamerListCar.Number = 0;
                gamerListCar.Map = Data.SettingCh.St.Car;

                Host.SendAllThread(gamerListCar);

            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Car = Data.SettingCh.St.Car;

                sendToHost.SendMessagesThread(1, Gamer);
            }

            e.Handled = true;

        }


        private void MapSettingGrid_KeyDown(object sender, KeyEventArgs e)
        {
            // Running host
            if (Data.HostOrClient)
            {

                if (e.Key == Key.Right || e.Key == Key.Up)
                {
                    Sound.PlaySound("carmap.mp3");

                    if (Data.SettingCh.St.Map != 10)
                        Data.SettingCh.St.Map++;
                    else
                        Data.SettingCh.St.Map = 0;
                }
                else if (e.Key == Key.Left || e.Key == Key.Down)
                {
                    Sound.PlaySound("carmap.mp3");

                    if (Data.SettingCh.St.Map != 0)
                        Data.SettingCh.St.Map--;
                    else
                        Data.SettingCh.St.Map = 10;

                }
                else if (e.Key == Key.Return)
                {
                    Sound.PlaySound("carmap.mp3");

                    MapMenu.IsOpen = true;

                    MapMenu.HorizontalOffset = -140;

                    MapMenu.Visibility = Visibility.Visible;

                    Keyboard.Focus(MapMenu);
                    return;
                }

                Get getMap = new Get();

                ShowMap(Data.SettingCh.St.Map);

                if (Data.HostOrClient)
                {
                    Data.GamersList.Map = Data.SettingCh.St.Map;
                    GamerList gamerListMap = new GamerList();
                    gamerListMap.Type = 1;
                    gamerListMap.Map = Data.SettingCh.St.Map;

                    Host.SendAllThread(gamerListMap);

                }

            }

            e.Handled = true;
        }

        private void CarSettingImgGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(CarSettingImgGrid);


            if (e.ChangedButton == MouseButton.Right)
            {
                Sound.PlaySound("carmap.mp3");

                if (Data.SettingCh.St.Car != 12)
                {
                    Data.SettingCh.St.Car++;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {
                    Data.SettingCh.St.Car = 0;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }



            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                Sound.PlaySound("carmap.mp3");

                if (Data.SettingCh.St.Car != 0)
                {
                    Data.SettingCh.St.Car--;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }

                else
                {

                    Data.SettingCh.St.Car = 12;
                    if (Data.HostOrClient)
                        Data.GamersList.gamer[0].GCar = Data.SettingCh.St.Car;
                }


            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                Sound.PlaySound("carmap.mp3");

                CarMenu.IsOpen = true;
                CarMenu.HorizontalOffset = -140;
                CarMenu.Visibility = Visibility.Visible;
                return;
            }

            ShowCar(Data.SettingCh.St.Car);
            Get _getCar = new Get();
            LineCar.Text = _getCar.GetCarToIndex(Data.SettingCh.St.Car);


            if (Data.HostOrClient)
            {

                GamerList gamerListCar = new GamerList();
                gamerListCar.Type = 2;
                gamerListCar.Number = 0;
                gamerListCar.Map = Data.SettingCh.St.Car;

                Host.SendAllThread(gamerListCar);

            }
            else
            {
                Gamers Gamer = new Gamers();
                Gamer.Car = Data.SettingCh.St.Car;

                sendToHost.SendMessagesThread(1, Gamer);
            }

            CarMenu.IsOpen = false;
            CarMenu.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void TextSend_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (Data.SettingCh.St.UseHotEnter && e.Key.ToString() == "Return"
                    && !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            { SendingMessage(); e.Handled = true; }

            else if (Data.SettingCh.St.UseHotEnterCtrl && Keyboard.IsKeyDown(Key.Enter)
                        && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            { SendingMessage(); e.Handled = true; }

            else if (Data.SettingCh.St.UseHotEnterShift && Keyboard.IsKeyDown(Key.Enter)
                        && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            { SendingMessage(); e.Handled = true; }
        }


    }
}
