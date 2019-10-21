using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Collections;

namespace TMC
{
    class GameSettings
    {
        
        // path to loader.cfg
        private string _tempPath;
        private string PathLoader
        {
            get
            {
                if (Data.SettingCh.St.LdPath != null)
                {
                    return _tempPath = Path.Combine(Data.SettingCh.St.LdPath, "loader.cfg");
                }
                else
                {
                    return null;
                }
            }
        }
        
        // Read into the variables all the parameters from the LdParameters file
        public string ReadParametrConfig(string parametr)
        {


            if (File.Exists(PathLoader))
            {

                using (StreamReader sr = new StreamReader(PathLoader))
                {
                    string[] line;
                    string oneLine;
                    while ((oneLine = sr.ReadLine()) != null)
                    {
                        line = oneLine.Split('=');

                        if (line.Length > 1)
                        {

                            line[0] = line[0].Trim();
                            line[1] = line[1].Trim();

                            if (line[0] == parametr)
                            {
                                return line[1];
                            }


                        }
                        else
                        {

                            line[0] = line[0].Trim();
                            if (line[0] == parametr)
                            {
                                return line[0];
                            }
                        }

                    }

                    return null;
                }
            }
            else
            {
                return null;
            }

        }


        public LdParameters ReadConfig()
        {

            LdParameters parameter = new LdParameters();

            if (File.Exists(PathLoader))
            {
                
                using (StreamReader sr = new StreamReader(PathLoader))
                {
                    string[] line;
                    string oneLine;

                    while ((oneLine = sr.ReadLine()) != null)
                    {
                        line = oneLine.Split('=');

                        line[0] = line[0].Trim();
                        if (line.Length > 1)
                            line[1] = line[1].Trim();



                        switch (line[0])
                        {

                            case "ddraw":
                                {
                                    parameter.ddraw = line[0];
                                    break;
                                }
                            case ";ddraw":
                                {
                                    parameter.ddraw = line[0];
                                    break;
                                }
                            case "newHUD":
                                {
                                    parameter.newHUD = line[0];
                                    break;
                                }
                            case ";newHUD":
                                {
                                    parameter.newHUD = line[0];
                                    break;
                                }
                            case "psCamera":
                                {

                                    parameter.psCamera = line[0];
                                    break;
                                }
                            case ";psCamera":
                                {

                                    parameter.psCamera = line[0];
                                    break;
                                }
                            case "ext":
                                {
                                    parameter.ext = line[0];
                                    break;
                                }
                            case ";ext":
                                {
                                    parameter.ext = line[0];
                                    break;
                                }
                            case "width":
                                {
                                    parameter.width = line[1];
                                    break;
                                }
                            case "height":
                                {
                                    parameter.height = line[1];
                                    break;
                                }
                            case "HUDSize":
                                {
                                    parameter.HUDSize = line[1];
                                    break;
                                }
                            case "RADARSize":
                                {
                                    parameter.RADARSize = line[1];
                                    break;
                                }
                            case "Name":
                                {
                                    parameter.Name = line[1];
                                    break;
                                }
                            case "Alias":
                                {

                                    parameter.Alias = line[1];
                                    break;
                                }
                            case "UDP":
                                {
                                    parameter.UDP = line[1];
                                    break;
                                }
                            case "lp":
                                {
                                    parameter.lp = line[1];
                                    break;
                                }
                            case "la":
                                {
                                    parameter.la = line[1];
                                    break;
                                }
                            case "ln":
                                {
                                    parameter.ln = line[1];
                                    break;
                                }
                            case "lr":
                                {
                                    parameter.lr = line[1];
                                    break;
                                }
                            case "server":
                                {
                                    parameter.server = line[1];
                                    break;
                                }
                            case "ls":
                                {
                                    parameter.ls = line[1];
                                    break;
                                }
                            case "lm":
                                {
                                    parameter.lm = line[1];
                                    break;
                                }
                            case "cd":
                                {
                                    parameter.cd = line[1];
                                    break;
                                }
                            case "ca":
                                {
                                    parameter.ca = line[1];
                                    break;
                                }
                            case "keyup":
                                {
                                    parameter.keyup = line[1];
                                    break;
                                }
                            case "keydown":
                                {
                                    parameter.keydown = line[1];
                                    break;
                                }
                            case "keyleft":
                                {
                                    parameter.keyleft = line[1];
                                    break;
                                }
                            case "keyright":
                                {
                                    parameter.keyright = line[1];
                                    break;
                                }
                            case "keycomboup":
                                {
                                    parameter.keycomboup = line[1];
                                    break;
                                }
                            case "keycombodown":
                                {
                                    parameter.keycombodown = line[1];
                                    break;
                                }
                            case "keycomboleft":
                                {
                                    parameter.keycomboleft = line[1];
                                    break;
                                }
                            case "keycomboright":
                                {
                                    parameter.keycomboright = line[1];
                                    break;
                                }
                            case "keyview1":
                                {
                                    parameter.keyview1 = line[1];
                                    break;
                                }
                            case "keyview2":
                                {
                                    parameter.keyview2 = line[1];
                                    break;
                                }
                            case "keybreak":
                                {
                                    parameter.keybreak = line[1];
                                    break;
                                }
                            case "keyturbo":
                                {
                                    parameter.keyturbo = line[1];
                                    break;
                                }
                            case "keytight":
                                {
                                    parameter.keytight = line[1];
                                    break;
                                }
                            case "keymgun":
                                {
                                    parameter.keymgun = line[1];
                                    break;
                                }
                            case "keyfire":
                                {
                                    parameter.keyfire = line[1];
                                    break;
                                }
                            case "keywnext":
                                {
                                    parameter.keywnext = line[1];
                                    break;
                                }
                            case "keywprev":
                                {
                                    parameter.keywprev = line[1];
                                    break;
                                }
                            case "keyscore":
                                {
                                    parameter.keyscore = line[1];
                                    break;
                                }
                            case "keyradar":
                                {
                                    parameter.keyradar = line[1];
                                    break;
                                }
                            case "keyw1":
                                {
                                    parameter.keyw1 = line[1];
                                    break;
                                }
                            case "keyw2":
                                {
                                    parameter.keyw2 = line[1];
                                    break;
                                }
                            case "keyw3":
                                {
                                    parameter.keyw3 = line[1];
                                    break;
                                }
                            case "keyw4":
                                {
                                    parameter.keyw4 = line[1];
                                    break;
                                }
                            case "keyw5":
                                {
                                    parameter.keyw5 = line[1];
                                    break;
                                }
                            case "keyw6":
                                {
                                    parameter.keyw6 = line[1];
                                    break;
                                }
                            case "keyw7":
                                {
                                    parameter.keyw7 = line[1];
                                    break;
                                }
                            case "keyw8":
                                {
                                    parameter.keyw8 = line[1];
                                    break;
                                }
                            default:
                                break;


                        }


                    }

                    return parameter;
                }
            }
            else
            {
                return null;
            }

        }


        // Change parameters changeParametr - the parameter we set
        // numberParametr - line number in which the parameter
        // use - if no, do not use, if yes - use if null is not involved

        public void ChangeConfig(string changeParametr, string LDparametr, string use)

        {
            List<string> lines = new List<string>();
            string line;
            string[] partLine;
            int countLine = 0;

            if (File.Exists(PathLoader))
            {

                // Read all the lines in a list
                using (StreamReader sr = new StreamReader(PathLoader))
                {

                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);

                    }
                    foreach (string firstLine in lines)
                    {
                        partLine = lines[countLine].Split('=');
                        partLine[0] = partLine[0].Replace(" ", "").Replace(";", "");

                        if (partLine[0] == LDparametr)
                        {
                            if (use == "no")
                            {
                                if (lines[countLine] != null)
                                {

                                    lines[countLine] = ";" + partLine[0];
                                    break;
                                }

                            }
                            else

                                if (use == "yes")
                            {
                                if (lines[countLine] != null)
                                {
                                    lines[countLine] = partLine[0];
                                    break;
                                }

                            }
                            else if (use == null)
                            {
                                lines[countLine] = partLine[0] + " = " + changeParametr + "";
                                break;
                            }

                        }
                        countLine++;
                    }

                }

                // Rewrote the file
                using (StreamWriter sw = new StreamWriter(PathLoader))
                {
                    foreach (string str in lines)
                    {
                        sw.WriteLine(str);
                    }

                    sw.Close();

                }

            }

        }

        public void WriteConfig(LdParameters parameter)
        {
            if (File.Exists(PathLoader))
            {


                // 
                using (StreamWriter sw = new StreamWriter(PathLoader))
                {
                    sw.WriteLine(parameter.ddraw);
                    sw.WriteLine("width = " + parameter.width);
                    sw.WriteLine("height = " + parameter.height);
                    sw.WriteLine(parameter.newHUD);
                    sw.WriteLine("HUDSize = " + parameter.HUDSize);
                    sw.WriteLine("RADARSize = " + parameter.RADARSize);
                    sw.WriteLine(parameter.psCamera);
                    sw.WriteLine("Name = " + parameter.Name);
                    sw.WriteLine("Alias = " + parameter.Alias);
                    sw.WriteLine(parameter.ext);
                    sw.WriteLine("UDP = " + parameter.UDP);
                    sw.WriteLine("lp = " + parameter.lp);
                    sw.WriteLine("la = " + parameter.la);
                    sw.WriteLine("ln = " + parameter.ln);
                    sw.WriteLine("lr = " + parameter.lr);
                    sw.WriteLine("server = " + parameter.server);
                    sw.WriteLine("ls = " + parameter.ls);
                    sw.WriteLine("lm = " + parameter.lm);
                    sw.WriteLine("cd = " + parameter.cd);
                    sw.WriteLine("ca = " + parameter.ca);
                    sw.WriteLine("keyup = " + parameter.keyup);
                    sw.WriteLine("keydown = " + parameter.keydown);
                    sw.WriteLine("keyleft = " + parameter.keyleft);
                    sw.WriteLine("keyright = " + parameter.keyright);
                    sw.WriteLine("keycomboup = " + parameter.keycomboup);
                    sw.WriteLine("keycombodown = " + parameter.keycombodown);
                    sw.WriteLine("keycomboleft = " + parameter.keycomboleft);
                    sw.WriteLine("keycomboright = " + parameter.keycomboright);
                    sw.WriteLine("keyview1 = " + parameter.keyview1);
                    sw.WriteLine("keyview2 = " + parameter.keyview2);
                    sw.WriteLine("keybreak = " + parameter.keybreak);
                    sw.WriteLine("keyturbo = " + parameter.keyturbo);
                    sw.WriteLine("keytight = " + parameter.keytight);
                    sw.WriteLine("keymgun = " + parameter.keymgun);
                    sw.WriteLine("keyfire = " + parameter.keyfire);
                    sw.WriteLine("keywnext = " + parameter.keywnext);
                    sw.WriteLine("keywprev = " + parameter.keywprev);
                    sw.WriteLine("keyscore = " + parameter.keyscore);
                    sw.WriteLine("keyradar = " + parameter.keyradar);
                    sw.WriteLine("keyw1 = " + parameter.keyw1);
                    sw.WriteLine("keyw2 = " + parameter.keyw2);
                    sw.WriteLine("keyw3 = " + parameter.keyw3);
                    sw.WriteLine("keyw4 = " + parameter.keyw4);
                    sw.WriteLine("keyw5 = " + parameter.keyw5);
                    sw.WriteLine("keyw6 = " + parameter.keyw6);
                    sw.WriteLine("keyw7 = " + parameter.keyw7);
                    sw.WriteLine("keyw8 = " + parameter.keyw8);

                    sw.Close();

                }

            }

        }
    }
}
