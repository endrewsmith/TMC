using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TMC
{
    public class Lobby
    {

        public int Number { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }

        public string Name { get; set; }



        public string Type { get; set; }

        public string Map { get; set; }

        public string Score { get; set; }

        public string Host { get; set; }

        public string Count { get; set; }

        public string CountOne { get; set; }

        public string Comment { get; set; }

        public string Status { get; set; }

        public string OpenCloseIngame { get; set; }

        public int IdTime { get; set; }

        public string Password { get; set; }

        public string PasswordInp { get; set; }

        public List<string> Players { get; set; }

        public string ModOrigin { get; set; }

        public string ImgPassYes
        {
            get
            {
                string path = Environment.CurrentDirectory + "\\data\\img\\contr\\pass_yes.gif";

                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    return null;
                }
            }
        }
        public string HostCheck
        {
            get
            {

                if (Host == Data.SettingCh.St.Name)
                {
                    return "lock";
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
