using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;

using System.Linq;

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
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;


namespace TMC
{

    [DataContract]
    public class Gamers : INotifyPropertyChanged
    {

        // type of message 0 - initialization, 1 - change of machine, 2 - commands, 3 - changed status, 4 - message
        [DataMember]
        public int Type { get; set; }

        public string Ping { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public int Car { get; set; }
        [DataMember]
        public int Team { get; set; }
        [DataMember]
        public IPEndPoint Udp { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Key { get; set; } // yes or no

        public string TeamColor { get; set; }

        public string GPing
        {
            get { return Ping; }
            set
            {
                Ping = value;
                OnPropertyChanged("Ping");
            }
        }

        public string GName
        {
            get { return Name; }
            set
            {
                Name = value;
                OnPropertyChanged("Name");
            }
        }

        public int GStatus
        {
            get { return Status; }
            set
            {
                Status = value;
                OnPropertyChanged("Status");
            }
        }

        public int GCar
        {
            get { return Car; }
            set
            {
                Car = value;
                OnPropertyChanged("Car");
            }
        }

        public int GTeam
        {
            get { return Team; }
            set
            {
                Team = value;
                OnPropertyChanged("Team");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }

    // Player List Class
    [DataContract]

    public class GamerList
    {
        [DataMember]
        public string IdSession { get; set; }
        // type of message -3 - room is closed, - 2 - wrong password, - 1 - name is already taken,
        // 0 - initialization, 1 - change, 2 - change of machine (command), 3 - change of status, 4 - message
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public int Map { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public string LobbyType { get; set; }
        [DataMember]
        public string ModOrig { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<Gamers> gamer { get; set; }

    }
}


