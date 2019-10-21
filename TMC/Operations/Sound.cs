using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace TMC
{
    class Sound
    {
        // Playing sound
        public static void PlaySound(string fileSound)
        {

            try

            {
                string pathSound = Environment.CurrentDirectory + "\\data\\sound\\" + fileSound;

                if (File.Exists(pathSound))

                {
                    if (Data.SettingCh.St.UseSound)
                    {
                        Data.SoundPlayer.Open(new Uri(pathSound, UriKind.Relative));
                        Data.SoundPlayer.Play();
                    }
                }

            }

            catch { }

        }
        // Playing sound
        public static void StopSound(string fileSound)
        {

            try

            {
                string pathSound = Environment.CurrentDirectory + "\\data\\sound\\" + fileSound;

                if (File.Exists(pathSound))

                {
                    if (Data.SettingCh.St.UseSound)
                    {
                        Data.SoundPlayer.Open(new Uri(pathSound, UriKind.Relative));
                        Data.SoundPlayer.Stop();
                    }
                }

            }

            catch { }

        }



    }
}
