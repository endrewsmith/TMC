using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace TMC
{
    public class Data
    {

        // Program version
        public static string Version = "ver 0.05";

        // Main windows
        public static MainWin MainWin;
        // Lobby window
        public static LobbyWin LobbyLink { get; set; }

        public static MediaPlayer SoundPlayer = new MediaPlayer();

        // Server or client running
        public static bool HostOrClient;

        // Static chat window link
        public static RichTextBox ChatStatic { get; set; }
        public static RichTextBox Inf { get; set; }
        public static Button BtnReady { get; set; }
        public static Button LineBtnReady { get; set; }
        public static Button BtnLineGo { get; set; }
        public static Button BtnLineUpd { get; set; }
        public static Button BtnLineClose { get; set; }
        public static Button Go { get; set; }
        public static Button GoLine { get; set; }
        public static DataGrid LobbyList { get; set; }

        // Cell selected in the lobby list
        public static int Selected { get; set; }

        public static ImageSource ImageMap { get; set; }

        public static Grid MainGridBlock { get; set; }

        // Static link to the settings variable
        public static SettingChange SettingCh = new SettingChange();

        // List of players with their parameters
        public static GamerList GamersList = new GamerList();

        public static ObservableCollection<Gamers> gamerList = new ObservableCollection<Gamers>();

        public static ObservableCollection<Lobby> webList = new ObservableCollection<Lobby>();

        // Open lobby or not
        public static bool LobbyOpen = true;

        // The lobby is busy, you need to turn off the function
        public static bool LobbyStart = false;

        // If true then connect to the server, if false then the local module
        public static bool JoinToServer;

        // Create on server or locally
        public static bool CreateToServer;

        // The lobby number I enter
        public static int LobbyNumber;

        // The number of the last logged-in player, needed to transmit information via UDP
        public static int LastNumber;

        // Monitors lobby update requests
        public static bool FollowToLobby;

        // Monitors the accepted lobby from the server
        public static bool FollowToGetLobby;

        public static string NameSession;

        public static string Comment;

        public static string LobbyTitle;

        public static string ModOrOrigin;

        public static int MaxGamers;

        public static bool UseLobbyPassword;

        public static string Password;

        public static string PasswordInput { get; set; }

        public static int MyNamber;

        public static int MyIp;

        public static string KeyForJoin;

        // UDP connection control
        public static bool UdpWorkRec;
        public static bool UdpWorkSend;
    }
}
