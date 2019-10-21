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

namespace TMC
{
    public class Set
    {


        // Starts the server and starts receiving clients
        public void SetText(string text)
        {
            try
            {
                Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.ChatStatic.Document.Blocks.Add(new Paragraph(new Run(text)));
                    Data.ChatStatic.ScrollToEnd();

                }));
            }
            catch { }
        }

        public void SetMassageInfo(string text)
        {
            try
            {
                Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    Paragraph paragraph = new Paragraph();
                    paragraph.TextAlignment = TextAlignment.Center;
                    paragraph.Inlines.Add(new Bold(new Run(text)));
                    Data.ChatStatic.Document.Blocks.Add(paragraph);
                    Data.ChatStatic.ScrollToEnd();
                }));
            }
            catch { }

        }

        public void SetMassage(int _from, string text)
        {
            Get _getColor = new Get();
            var bc = new BrushConverter();

            Data.Inf.Dispatcher.Invoke(new Action(delegate ()
            {
                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Left;
                paragraph.Inlines.Add(new Bold(new Run(Data.GamersList.gamer[_from].Name + ": "))
                {
                    Foreground = (Brush)bc.ConvertFrom(_getColor.GetColorGamer(_from))
                });
                paragraph.Inlines.Add(text);
                Data.ChatStatic.Document.Blocks.Add(paragraph);
                Data.ChatStatic.ScrollToEnd();
            }));

        }

        public void SetMassageCar(int _from)
        {
            Get getColor = new Get();
            var bc = new BrushConverter();

            Data.Inf.Dispatcher.Invoke(new Action(delegate ()
            {
                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Right;
                paragraph.Inlines.Add(new Italic(new Run("*machine is already selected by "))
                {
                    Foreground = (Brush)bc.ConvertFrom("#222222")
                });
                paragraph.Inlines.Add(new Italic(new Run(Data.GamersList.gamer[_from].Name))
                {
                    Foreground = (Brush)bc.ConvertFrom(getColor.GetColorGamer(_from))
                });

                Data.ChatStatic.Document.Blocks.Add(paragraph);
                Data.ChatStatic.ScrollToEnd();
            }));

        }

        public void SetMassageAddLeave(bool addLeave, int from)
        {
            Get _getColor = new Get();
            var bc = new BrushConverter();
            try
            {
                if (from > 0 && Data.GamersList.gamer.Count > from)
                {
                    Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    Paragraph paragraph = new Paragraph();
                    paragraph.TextAlignment = TextAlignment.Right;
                    string add = "";
                    if (addLeave)
                    {
                        add = " joined to the lobby";
                        Sound.PlaySound("welcome.mp3");
                    }
                    else
                    {
                        add = " left the lobby";
                        Sound.PlaySound("leave.mp3");
                    }

                    paragraph.Inlines.Add(new Italic(new Run(Data.GamersList.gamer[from].Name))
                    {
                        Foreground = (Brush)bc.ConvertFrom(_getColor.GetColorGamer(from))
                    });
                    paragraph.Inlines.Add(new Italic(new Run(add))
                    {
                        Foreground = (Brush)bc.ConvertFrom("#222222")
                    });

                //paragraph.Inlines.Add(text);
                Data.ChatStatic.Document.Blocks.Add(paragraph);
                    Data.ChatStatic.ScrollToEnd();
                }));
                }
            }
            catch { }
        }

        public void SetTextLink(string text, string hlink)
        {

            Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
            {
                FlowDocument document = new FlowDocument();
                Paragraph paragraph = new Paragraph();
                Hyperlink link = new Hyperlink(new Run(text));
                link.NavigateUri = new Uri(hlink);
                link.RequestNavigate += new RequestNavigateEventHandler(delegate (object obj, RequestNavigateEventArgs nav)
                {
                    //Process.Start("IExplore.exe", nav.Uri.OriginalString);
                });
                paragraph.Inlines.Add(link);
                document.Blocks.Add(paragraph);
                Data.ChatStatic.Document = document;
                Data.ChatStatic.ScrollToEnd();
            }));
        }

        public void SetInfo(int index)
        {
            var bc = new BrushConverter();

            if (Data.webList.Count > 0)
            {
                Data.Selected = index;
                string players = "";

                foreach (string part in Data.webList[index].Players)
                {

                    players += part + ",";
                }
                players = players.Substring(0, players.Length - 1);

                string version = "";
                if (Data.webList[index].ModOrigin == "Orig") { version = "Original"; }
                else { version = "Modified"; }

                try
                {
                    Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                    {
                        Data.Inf.Document.Blocks.Clear();
                        Paragraph paragraph = new Paragraph();
                        paragraph.TextAlignment = TextAlignment.Left;
                        paragraph.Foreground = (Brush)bc.ConvertFrom("#222222");
                        paragraph.Inlines.Add(new Bold(new Run("Lobby name:")));
                        paragraph.Inlines.Add(" . . . . . . . . . . " + Data.webList[index].Name + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Number of players:")));
                        paragraph.Inlines.Add(" . . . . . " + Data.webList[index].Count + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Map: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . . . . . . . " + Data.webList[index].Map + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Number of deaths: ")));
                        paragraph.Inlines.Add(" . . . . . " + Data.webList[index].Score + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Player list: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . . . " + players + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Type: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . . . . . . . " + Data.webList[index].Type + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Version: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . . . . . " + version + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Password: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . . . " + Data.webList[index].Password + "\r\n");
                        paragraph.Inlines.Add(new Bold(new Run("Information: ")));
                        paragraph.Inlines.Add(" . . . . . . . . . . " + Data.webList[index].Comment + "\r\n");
                        Data.Inf.Document.Blocks.Add(paragraph);

                    }));
                }
                catch { }
            }
            else
            {
                Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.Inf.Document.Blocks.Clear();

                }));
            }
        }

        public void SetSomeInfo(string message)
        {
            var bc = new BrushConverter();

            try
            {
                Data.Inf.Dispatcher.Invoke(new Action(delegate ()
                {
                    Data.Inf.Document.Blocks.Clear();
                    Paragraph paragraph = new Paragraph();
                    paragraph.TextAlignment = TextAlignment.Left;
                    paragraph.Foreground = (Brush)bc.ConvertFrom("#222222");

                    paragraph.Inlines.Add(message);

                    Data.Inf.Document.Blocks.Add(paragraph);

                }));
            }
            catch { }
        }

        public void AddToGrid(int numberGamer)
        {

            // Add tables from another stream to the collection
            Data.ChatStatic.Dispatcher.Invoke(new Action(delegate ()
            {
                Data.gamerList.Add(Data.GamersList.gamer[numberGamer]);

            }));
        }

    }
}
