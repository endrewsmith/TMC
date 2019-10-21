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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.IO;

namespace TMC
{

    /// <summary>
    /// Interaction logic for GameSett.xaml
    /// </summary>
    public partial class GameSett : Window
    {

        LdParameters tempParameters = new LdParameters();

        private Image _imageScreen = new Image();
        private ImageBrush _myBrush = new ImageBrush();

        private Image _imageForRadar = new Image();
        private ImageBrush _myBrushRadar = new ImageBrush();

        private Image _imageHUD = new Image();
        private ImageBrush _myBrushHUD = new ImageBrush();

        private bool _radarKey = false;

        // To get possible monitor resolutions
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;
        const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20; private const int CCHFORMNAME = 0x20;[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra; public int dmFields; public int dmPositionX; public int dmPositionY; public int dmDisplayOrientation; public int dmDisplayFixedOutput; public short dmColor; public short dmDuplex; public short dmYResolution; public short dmTTOption; public short dmCollate;[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)] public string dmFormName; public short dmLogPixels; public int dmBitsPerPel; public int dmPelsWidth; public int dmPelsHeight; public int dmDisplayFlags; public int dmDisplayFrequency; public int dmICMMethod; public int dmICMIntent; public int dmMediaType; public int dmDitherType; public int dmReserved1; public int dmReserved2; public int dmPanningWidth; public int dmPanningHeight;
        }

        public Dictionary<string, string> Key;

        // Recording Settings
        private void WriteSetting()
        {

            Data.SettingCh.St.GameSettings_X_pos = this.Left;
            Data.SettingCh.St.GameSettings_Y_pos = this.Top;
            Data.SettingCh.St.GameSettings_Width = this.Width;
            Data.SettingCh.St.GameSettings_Height = this.Height;
            if (WindowState == WindowState.Maximized)
            {
                Data.SettingCh.St.GameSettingsMaximized = true;
            }
            else
            {
                Data.SettingCh.St.GameSettingsMaximized = false;
            }

            Data.SettingCh.WriteXml();
        }

        // Reading settings
        private void ReadSetting()
        {

            Data.SettingCh.ReadXml();

            this.Left = Data.SettingCh.St.GameSettings_X_pos;
            this.Top = Data.SettingCh.St.GameSettings_Y_pos;
            this.Width = Data.SettingCh.St.GameSettings_Width;
            this.Height = Data.SettingCh.St.GameSettings_Height;
            if (Data.SettingCh.St.GameSettingsMaximized)
            {
                WindowState = WindowState.Maximized;
            }

            SetResolution();
        }

        // Setting permission
        private void SetResolution()
        {
            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            bool trigger;
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {

                trigger = true;

                if (vDevMode.dmDisplayFrequency == 60)
                {
                    foreach (var item in ResolutionType.Items)
                    {
                        if (item.ToString() == vDevMode.dmPelsWidth + "x" + vDevMode.dmPelsHeight)
                        {
                            trigger = false;
                            break;
                        }

                    }
                    if (trigger)
                    {
                        ResolutionType.Items.Add(vDevMode.dmPelsWidth + "x" + vDevMode.dmPelsHeight);
                    }

                }

                i++;
            }

        }


        // Writing settings to the loader
        private int GetResolution(string resolution, int widthHeight)
        {
            string[] parts;
            parts = resolution.Split('x');
            int Int32 = 0;
            switch (widthHeight)
            {
                case 0:
                    {

                        Int32.TryParse(parts[0], out Int32);
                        return Int32;
                    }
                case 1:
                    {
                        Int32.TryParse(parts[1], out Int32);
                        return Int32;
                    }
                default:
                    return -1;
            }
        }

        // Writing settings to the loader
        private void WriteInLoader()
        {
            GameSettings wr = new GameSettings();
            LdParameters parameter = new LdParameters();

            // 1. Сhange ddraw
            if ((bool)EnableDirectDraw.IsChecked)
            {
                parameter.ddraw = "ddraw";
            }
            else
            {
                parameter.ddraw = ";ddraw";
            }
            // 2. width
            parameter.width = GetResolution(ResolutionType.SelectedItem.ToString(), 0).ToString();
            // 3. height
            parameter.height = GetResolution(ResolutionType.SelectedItem.ToString(), 1).ToString();

            // 4. newHUD
            if ((bool)newHUD.IsChecked)
            {
                parameter.newHUD = "newHUD";
            }
            else
            {
                parameter.newHUD = ";newHUD";
            }
            // 5. HUDSize
            parameter.HUDSize = HudTxt.Text;
            // 6. RADARSize
            parameter.RADARSize = RadarTxt.Text;


            // 7. psCamera
            ComboBoxItem typeItemCam = (ComboBoxItem)Camera.SelectedItem;
            string valueCam = typeItemCam.Content.ToString();
            if (valueCam == "PC")
            {
                parameter.psCamera = ";psCamera";
            }
            else
            {
                parameter.psCamera = "psCamera";
            }
            // 8. Name 
            parameter.Name = tempParameters.Name;
            // 9. Alias
            parameter.Alias = Alias.Text;
            // 10. ext
            parameter.ext = "ext";
            // 11. UDP
            parameter.UDP = tempParameters.UDP;
            // 12. lp
            parameter.lp = tempParameters.lp;
            // 13. la
            parameter.la = tempParameters.la;
            // 14. ln
            parameter.ln = tempParameters.ln;
            // 15. lr
            parameter.lr = tempParameters.lr;
            // 16. server
            parameter.server = tempParameters.server;
            // 17. ls
            parameter.ls = tempParameters.ls;
            // 18. lm
            parameter.lm = tempParameters.lm;
            // 19. cd
            parameter.cd = tempParameters.cd;
            // 20. ca
            parameter.ca = tempParameters.ca;

            // Keys setting (21 - 47)
            parameter.keyup = Key.FirstOrDefault(x => x.Value == Key1.Text).Key;
            parameter.keydown = Key.FirstOrDefault(x => x.Value == Key2.Text).Key;
            parameter.keyleft = Key.FirstOrDefault(x => x.Value == Key3.Text).Key;
            parameter.keyright = Key.FirstOrDefault(x => x.Value == Key4.Text).Key;
            parameter.keycomboup = Key.FirstOrDefault(x => x.Value == Key4.Text).Key;

            parameter.keycomboup = Key.FirstOrDefault(x => x.Value == Key5.Text).Key;
            parameter.keycombodown = Key.FirstOrDefault(x => x.Value == Key6.Text).Key;
            parameter.keycomboleft = Key.FirstOrDefault(x => x.Value == Key7.Text).Key;
            parameter.keycomboright = Key.FirstOrDefault(x => x.Value == Key8.Text).Key;
            parameter.keyview1 = Key.FirstOrDefault(x => x.Value == Key9.Text).Key;
            parameter.keyview2 = Key.FirstOrDefault(x => x.Value == Key10.Text).Key;
            parameter.keybreak = Key.FirstOrDefault(x => x.Value == Key11.Text).Key;
            parameter.keyturbo = Key.FirstOrDefault(x => x.Value == Key12.Text).Key;
            parameter.keytight = Key.FirstOrDefault(x => x.Value == Key13.Text).Key;
            parameter.keymgun = Key.FirstOrDefault(x => x.Value == Key14.Text).Key;
            parameter.keyfire = Key.FirstOrDefault(x => x.Value == Key15.Text).Key;
            parameter.keywnext = Key.FirstOrDefault(x => x.Value == Key16.Text).Key;
            parameter.keywprev = Key.FirstOrDefault(x => x.Value == Key17.Text).Key;
            parameter.keyscore = Key.FirstOrDefault(x => x.Value == Key18.Text).Key;
            parameter.keyradar = Key.FirstOrDefault(x => x.Value == Key19.Text).Key;
            parameter.keyw1 = Key.FirstOrDefault(x => x.Value == Key20.Text).Key;
            parameter.keyw2 = Key.FirstOrDefault(x => x.Value == Key21.Text).Key;
            parameter.keyw3 = Key.FirstOrDefault(x => x.Value == Key22.Text).Key;
            parameter.keyw4 = Key.FirstOrDefault(x => x.Value == Key23.Text).Key;
            parameter.keyw5 = Key.FirstOrDefault(x => x.Value == Key24.Text).Key;
            parameter.keyw6 = Key.FirstOrDefault(x => x.Value == Key25.Text).Key;
            parameter.keyw7 = Key.FirstOrDefault(x => x.Value == Key26.Text).Key;
            parameter.keyw8 = Key.FirstOrDefault(x => x.Value == Key27.Text).Key;

            wr.WriteConfig(parameter);

        }

        // Read the settings from the loader
        private void ReadInLoader()
        {

            GameSettings sr = new GameSettings();
            List<string> tmconfig1 = new List<string>();

            tempParameters = sr.ReadConfig();


            try
            {
                if (tempParameters.ddraw != null)
                {
                    if (tempParameters.ddraw.StartsWith(";"))
                    {
                        EnableDirectDraw.IsChecked = false;
                        ResolutionType.IsEnabled = false;
                    }
                    else
                    {
                        EnableDirectDraw.IsChecked = true;
                        ResolutionType.IsEnabled = true;
                    }
                }

                try
                {
                    ResolutionType.SelectedItem = ResolutionType.Items[ResolutionType.Items.IndexOf(tempParameters.width + "x" + tempParameters.height)];
                }
                catch { }

                if (tempParameters.newHUD.StartsWith(";"))
                {
                    newHUD.IsChecked = false;
                    View.IsEnabled = false;
                }
                else
                {
                    newHUD.IsChecked = true;
                    View.IsEnabled = true;
                }

                Alias.Text = tempParameters.Alias;

                // Сhange camera
                ComboBoxItem typeItemCam = (ComboBoxItem)Camera.SelectedItem;
                if (tempParameters.psCamera.StartsWith(";"))
                {
                    Camera.SelectedIndex = 0;
                }
                else
                {
                    Camera.SelectedIndex = 1;
                }


            }
            catch { }


            //Keys setting
            try
            {

                try { Key1.Text = Key[tempParameters.keyup]; } catch { }
                try
                {
                    Key2.Text = Key[tempParameters.keydown];
                }
                catch { }
                try
                { Key3.Text = Key[tempParameters.keyleft]; }
                catch { }
                try
                {
                    Key4.Text = Key[tempParameters.keyright];

                    try
                    {
                        Key5.Text = Key[tempParameters.keycomboup];
                    }
                    catch { }
                    try
                    {
                        Key6.Text = Key[tempParameters.keycombodown];
                    }
                    catch { }
                    try { Key7.Text = Key[tempParameters.keycomboleft]; } catch { }
                    try
                    {
                        Key8.Text = Key[tempParameters.keycomboright];
                    }
                    catch { }
                    try
                    {
                        Key9.Text = Key[tempParameters.keyview1];
                    }
                    catch { }
                    try
                    { Key10.Text = Key[tempParameters.keyview2]; }
                    catch { }
                }
                catch { }

                try
                {
                    Key11.Text = Key[tempParameters.keybreak];
                }
                catch { }
                try
                {
                    Key12.Text = Key[tempParameters.keyturbo];
                }
                catch { }
                try
                {
                    Key13.Text = Key[tempParameters.keytight];
                }
                catch { }
                try
                {
                    Key14.Text = Key[tempParameters.keymgun];
                }
                catch { }
                try
                {
                    Key15.Text = Key[tempParameters.keyfire];
                }
                catch { }
                try
                {
                    Key16.Text = Key[tempParameters.keywnext];
                }
                catch { }
                try
                {
                    Key17.Text = Key[tempParameters.keywprev];
                }
                catch { }
                try
                {
                    Key18.Text = Key[tempParameters.keyscore];
                }
                catch { }
                try
                {
                    Key19.Text = Key[tempParameters.keyradar];
                }
                catch { }
                try
                {
                    Key20.Text = Key[tempParameters.keyw1];
                }
                catch { }
                try
                {
                    Key21.Text = Key[tempParameters.keyw2];
                }
                catch { }
                try
                {
                    Key22.Text = Key[tempParameters.keyw3];
                }
                catch { }
                try
                {
                    Key23.Text = Key[tempParameters.keyw4];
                }
                catch { }
                try
                {
                    Key24.Text = Key[tempParameters.keyw5];
                }
                catch { }
                try
                {
                    Key25.Text = Key[tempParameters.keyw6];
                }
                catch { }
                try
                { Key26.Text = Key[tempParameters.keyw7]; }
                catch { }
                try
                { Key27.Text = Key[tempParameters.keyw8]; }
                catch { }
            }
            catch { }

            // View
            try
            {
                HudTxt.Text = tempParameters.HUDSize;
                RadarTxt.Text = tempParameters.RADARSize;
            }
            catch { }


        }

        // Display card by number
        public void ShowScreen()
        {

            string path = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "screen.gif";
            string pathRadar = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "radar.gif";
            string pathHUD = Environment.CurrentDirectory + "\\data\\img\\contr\\" + "HUD.gif";

            if (!Data.SettingCh.St.UseBottomhModule)
            {
                if (File.Exists(path) && File.Exists(pathRadar) && File.Exists(pathHUD))
                {

                    _imageScreen.Source = new BitmapImage(new Uri(path));
                    _myBrush.ImageSource = _imageScreen.Source;
                    ScreenSettingGrid.Background = _myBrush;

                    _imageForRadar.Source = new BitmapImage(new Uri(pathRadar));
                    _myBrushRadar.ImageSource = _imageForRadar.Source;
                    ScreenSettingRadar.Background = _myBrushRadar;

                    _imageHUD.Source = new BitmapImage(new Uri(pathHUD));
                    _myBrushHUD.ImageSource = _imageHUD.Source;
                    ScreenSettingHUD.Background = _myBrushHUD;

                    double w = 0, h = 0, hw = 0;
                    if (ResolutionType.SelectedItem != null)
                    {
                        w = Convert.ToDouble(GetResolution(ResolutionType.SelectedItem.ToString(), 0));
                        h = Convert.ToDouble(GetResolution(ResolutionType.SelectedItem.ToString(), 1));
                        hw = h / w;
                    }

                    ScreenSettingGrid.Height = (KeySet.ActualWidth - 6) * hw; //KeySet.ActualWidth-6
                    ScreenSettingGrid.Width = KeySet.ActualWidth - 6;

                    double hwTemp = ScreenSettingGrid.Height / h;

                    int Int32 = 0;
                    Int32.TryParse(RadarTxt.Text, out Int32);

                    double coefRadar = Convert.ToDouble(Int32);
                    coefRadar = coefRadar / 100;

                    Int32.TryParse(HudTxt.Text, out Int32);
                    double coefHUD = Convert.ToDouble(Int32);
                    coefHUD = coefHUD / 100;

                    ScreenSettingRadar.Height = 200 * coefRadar * hwTemp;
                    ScreenSettingRadar.Width = 240 * coefRadar * hwTemp;

                    ScreenSettingHUD.Height = 170 * coefHUD * hwTemp;
                    ScreenSettingHUD.Width = 140 * coefHUD * hwTemp;

                    _radarKey = true;
                }
            }
        }

        public GameSett()
        {
            InitializeComponent();
            Key = new Dictionary<string, string>();
            Key.Add("1", "Escape");
            Key.Add("2", "D1");
            Key.Add("3", "D2");
            Key.Add("4", "D3");
            Key.Add("5", "D4");
            Key.Add("6", "D5");
            Key.Add("7", "D6");
            Key.Add("8", "D7");
            Key.Add("9", "D8");
            Key.Add("10", "D9");
            Key.Add("11", "D0");
            Key.Add("12", "OemMinus");
            Key.Add("13", "OemPlus");
            Key.Add("14", "Back");
            Key.Add("15", "Tab");
            Key.Add("16", "Q");
            Key.Add("17", "W");
            Key.Add("18", "E");
            Key.Add("19", "R");
            Key.Add("20", "T");
            Key.Add("21", "Y");
            Key.Add("22", "U");
            Key.Add("23", "I");
            Key.Add("24", "O");
            Key.Add("25", "P");
            Key.Add("26", "OemOpenBrackets");
            Key.Add("27", "Oem6");
            Key.Add("28", "Return");
            Key.Add("29", "LeftCtrl");
            Key.Add("30", "A");
            Key.Add("31", "S");
            Key.Add("32", "D");
            Key.Add("33", "F");
            Key.Add("34", "G");
            Key.Add("35", "H");
            Key.Add("36", "J");
            Key.Add("37", "K");
            Key.Add("38", "L");
            Key.Add("39", "Oem1");
            Key.Add("40", "OemQuotes");
            Key.Add("41", "em3");
            Key.Add("42", "LeftShift");
            Key.Add("43", "Oem5");
            Key.Add("44", "Z");
            Key.Add("45", "X");
            Key.Add("46", "C");
            Key.Add("47", "V");
            Key.Add("48", "B");
            Key.Add("49", "N");
            Key.Add("50", "M");
            Key.Add("51", "OemComma");
            Key.Add("52", "OemPeriod");
            Key.Add("53", "OemQuestion");
            Key.Add("54", "RightShift");
            Key.Add("55", "Multiply");
            Key.Add("56", "System");
            Key.Add("57", "Space");
            Key.Add("58", "Capital");
            Key.Add("59", "F1");
            Key.Add("60", "F2");
            Key.Add("61", "F3");
            Key.Add("62", "F4");
            Key.Add("63", "F5");
            Key.Add("64", "F6");
            Key.Add("65", "F7");
            Key.Add("66", "F8");
            Key.Add("67", "F9");
            Key.Add("68", "F10");
            Key.Add("69", "NumLock");
            Key.Add("70", "Scroll");
            Key.Add("71", "NumPad7");
            Key.Add("72", "NumPad8");
            Key.Add("73", "NumPad9");
            Key.Add("74", "Subtract");
            Key.Add("75", "NumPad4");
            Key.Add("76", "NumPad5");
            Key.Add("77", "NumPad6");
            Key.Add("78", "Add");
            Key.Add("79", "NumPad1");
            Key.Add("80", "NumPad2");
            Key.Add("81", "NumPad3");
            Key.Add("82", "NumPad0");
            Key.Add("83", "Decimal");
            Key.Add("84", "F11");
            Key.Add("85", "F12");
            Key.Add("156", "Return");
            Key.Add("157", "RightCtrl");
            Key.Add("88", "Divide");
            Key.Add("183", "Sys Rq");
            Key.Add("90", "System");
            Key.Add("197", "Pause");
            Key.Add("199", "Home");
            Key.Add("200", "Up");
            Key.Add("201", "PageUp");
            Key.Add("203", "Left");
            Key.Add("205", "Right");
            Key.Add("207", "End");
            Key.Add("208", "Down");
            Key.Add("209", "Next");
            Key.Add("210", "Insert");
            Key.Add("211", "Delete");

            ReadSetting();

        }

        private void Key1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key1.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key2.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key3_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key3.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key4_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key4.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key5_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key5.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key6_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key6.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key7_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key7.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key8_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key8.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key9_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key9.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key10_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key10.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key11_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key11.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key12_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key12.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key13_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key13.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key14_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key14.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key15_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key15.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key16_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key16.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key17_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key17.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key18_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key18.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key19_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key19.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key20_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key20.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key21_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key21.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key22_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key22.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key23_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key23.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key24_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key24.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key25_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key25.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key26_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key26.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Key27_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key27.Text = e.Key.ToString();
            Keyboard.ClearFocus();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            WriteSetting();
            WriteInLoader();
        }

        private void EnableDirectDraw_Checked(object sender, RoutedEventArgs e)
        {
            ResolutionType.IsEnabled = true;
        }

        private void NewHUD_Checked(object sender, RoutedEventArgs e)
        {
            View.IsEnabled = true;
        }

        private void EnableDirectDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            ResolutionType.IsEnabled = false;
        }

        private void NewHUD_Unchecked(object sender, RoutedEventArgs e)
        {
            View.IsEnabled = false;
        }

        private void Setting_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.ActualHeight < 200)
                this.Height = 480;
        }

        private void KeySet_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.ActualHeight < 200)
                this.Height = 480;
        }

        private void View_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.ActualHeight < 200)
                this.Height = 480;

            ShowScreen();
        }

        private void ResolutionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowScreen();
        }

        private void HudTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_radarKey)
                ShowScreen();
        }

        private void RadarTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_radarKey)
                ShowScreen();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadInLoader();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_radarKey)
                ShowScreen();
        }
    }
}
