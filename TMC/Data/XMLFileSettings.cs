using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Надо добавить для работы класса
using System.Xml.Serialization;
using System.IO;


using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace TMC
{

    // Application window settings class
    public class Settings
    {
        // Path to settings file
        public String XMLFileName = Environment.CurrentDirectory + "\\settings.xml";

        // Main Window Settings

        public double TMC_X_pos; // X coordinate of the main window
        public double TMC_Y_pos; // Y coordinate of the main window
        public double TMC_Width; // Main window width
        public double TMC_Height; // Main window height
        public bool Maximized; // Maximized window or minimized
        public double MenuGridHeight; // Menu Separator Position
        public double InfoGridHeight; // Position of list separator and information
        public bool ShowLobbySetting;

        // Sorting the list of games

        public bool Sort1;
        public bool Sort2;
        public bool Sort3;
        public bool Sort4;

        // Settings window "Settings"

        public double Settings_X_pos; // X coordinate of the main window
        public double Settings_Y_pos; // Y coordinate of the main window
        public double Settings_Width; // Width
        public double Settings_Height; // Height
        public bool SettingsMaximized; // Maximized window or minimized

        public string Server; // server link
        public string LdPath; // way to ld
        public string Name; // Player name
        public string TcpPort; // имя игрока
        public string UdpPort; // имя игрока
        public string IP;
        public bool UseAutoIP;


        // Lobby creation options
        public string NameLobby;
        public bool UseMod;
        public int TypeGame;
        public bool UsePassword;
        public string Password;
        public string Comment;
        public string Key;


        // View window settings

        public double View_X_pos; 
        public double View_Y_pos; 
        public double View_Width; 
        public double View_Height; 
        public bool ViewMaximized; 

        public bool UseLocalModule;
        public bool UseTopMenu;
        public bool UseBouthModule;
        public bool UseTopModule;
        public bool UseBottomhModule;
        public bool UseSound;
        public bool UseHotEnter;
        public bool UseHotEnterCtrl;
        public bool UseHotEnterShift;

        // Lobby Window Settings

        public double Lobby_X_pos; 
        public double Lobby_Y_pos; 
        public double Lobby_Width; 
        public double Lobby_Height; 
        public bool LobbyMaximized; 

        public double LobbyGridsp1; // Separator position 1
        public double LobbyGridsp2; // Separator position 2
        public double LobbyGridsp3; // Separator position 3
        public double LobbyGridsp4; // Separator position 4

        // Lobby Game Settings

        public int Map = 0;
        public int Car = 0;
        public int Score = 0;
        public int Team = 0;

        // Game Settings Window Settings

        public double GameSettings_X_pos; 
        public double GameSettings_Y_pos; 
        public double GameSettings_Width; 
        public double GameSettings_Height; 
        public bool GameSettingsMaximized; 
    }

    // Class of work with settings
    public class SettingChange
    {
        public Settings St;

        public SettingChange()
        {
            St = new Settings();
        }

        // Save settings to a file
        public void WriteXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter(St.XMLFileName);
            ser.Serialize(writer, St);
            writer.Close();
        }

        // Reading settings from a file
        public void ReadXml()
        {
            if (File.Exists(St.XMLFileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                TextReader reader = new StreamReader(St.XMLFileName);
                St = ser.Deserialize(reader) as Settings;
                reader.Close();
            }

            // If there is no file, then create with default settings
            else

            {
                Settings setting = new Settings();

                // Main Window Settings

                St.TMC_X_pos = 0;           // X coordinate of the main window
                St.TMC_Y_pos = 0;           // Y coordinate of the main window
                St.TMC_Width = 550;         // Width
                St.TMC_Height = 480;        // Height
                St.Maximized = false;       // maximized window or minimized
                St.MenuGridHeight = 30;     // Menu Separator Position
                St.ShowLobbySetting = false;


                St.Settings_X_pos = 0; 
                St.Settings_Y_pos = 0; 
                St.Settings_Width = 450; 
                St.Settings_Height = 390; 
                St.SettingsMaximized = false; 

                // Lobby Window Settings

                St.Lobby_X_pos = 0; 
                St.Lobby_Y_pos = 0; 
                St.Lobby_Width = 580; 
                St.Lobby_Height = 540; 
                St.LobbyMaximized = false; 

                St.LobbyGridsp1 = 164; 
                St.LobbyGridsp2 = 168; 
                St.LobbyGridsp3 = 290; 
                St.LobbyGridsp4 = 210; 

                // View window settings

                St.View_X_pos = 0; 
                St.View_Y_pos = 0; 
                St.View_Width = 470; 
                St.View_Height = 410; 
                St.ViewMaximized = false; 

                St.UseLocalModule = false;
                St.UseTopMenu = true;
                St.UseBouthModule = true;
                St.UseTopModule = false;
                St.UseBottomhModule = false;
                St.UseSound = true;
                St.UseMod = true;
                St.UseHotEnter = true;
                St.UseHotEnterCtrl = true;
                St.UseHotEnterShift = true;



                St.Sort1 = false;
                St.Sort2 = false;
                St.Sort3 = false;
                St.Sort4 = false;

                St.GameSettings_X_pos = 0; 
                St.GameSettings_Y_pos = 0; 
                St.GameSettings_Width = 390; 
                St.GameSettings_Height = 520; 
                St.GameSettingsMaximized = false; 



                FileInfo MyFile = new FileInfo(St.XMLFileName);
                FileStream fs = MyFile.Create();
                fs.Close();

                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                TextWriter writer = new StreamWriter(St.XMLFileName);
                ser.Serialize(writer, St);
                writer.Close();


            }
        }
    }
}
