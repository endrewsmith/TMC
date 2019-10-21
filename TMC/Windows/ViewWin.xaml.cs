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

namespace TMC
{
    /// <summary>
    /// Логика взаимодействия для ViewWin.xaml
    /// </summary>
    /// 



    public partial class ViewWin : Window
    {


        private void WriteSetting()
        {
            Data.SettingCh.St.View_X_pos = this.Left;
            Data.SettingCh.St.View_Y_pos = this.Top;
            Data.SettingCh.St.View_Width = this.Width;
            Data.SettingCh.St.View_Height = this.Height;
            if (WindowState == WindowState.Maximized)
            {
                Data.SettingCh.St.ViewMaximized = true;
            }
            else
            {
                Data.SettingCh.St.ViewMaximized = false;
            }
            // Set up checkboxes
            if ((bool)UseLocalM.IsChecked)
            {
                Data.SettingCh.St.UseLocalModule = true;
            }
            else
            {
                Data.SettingCh.St.UseLocalModule = false;
            }

            if ((bool)UseTopMenu.IsChecked)
            {
                Data.SettingCh.St.UseTopMenu = true;
            }
            else
            {
                Data.SettingCh.St.UseTopMenu = false;
            }

            if ((bool)UseBouthM.IsChecked)
            {
                Data.SettingCh.St.UseBouthModule = true;
            }
            else
            {
                Data.SettingCh.St.UseBouthModule = false;
            }
  
            if ((bool)UseTopM.IsChecked)
            {
                Data.SettingCh.St.UseTopModule = true;
            }
            else
            {
                Data.SettingCh.St.UseTopModule = false;
            }

            if ((bool)UseBottomM.IsChecked)
            {
                Data.SettingCh.St.UseBottomhModule = true;
            }
            else
            {
                Data.SettingCh.St.UseBottomhModule = false;
            }
 
            if ((bool)UseSound.IsChecked)
            {
                Data.SettingCh.St.UseSound = false;
            }
            else
            {
                Data.SettingCh.St.UseSound = true;
            }
 
            if ((bool)UseHotEnter.IsChecked)
            {
                Data.SettingCh.St.UseHotEnter = true;
            }
            else
            {
                Data.SettingCh.St.UseHotEnter = false;
            }
            if ((bool)UseHotEnterCtrl.IsChecked)
            {
                Data.SettingCh.St.UseHotEnterCtrl = true;
            }
            else
            {
                Data.SettingCh.St.UseHotEnterCtrl = false;
            }
            if ((bool)UseHotEnterShift.IsChecked)
            {
                Data.SettingCh.St.UseHotEnterShift = true;
            }
            else
            {
                Data.SettingCh.St.UseHotEnterShift = false;
            }

            Data.SettingCh.WriteXml();
        }

        // Reading settings
        private void ReadSetting()
        {
            Data.SettingCh.ReadXml();

            this.Left = Data.SettingCh.St.View_X_pos;
            this.Top = Data.SettingCh.St.View_Y_pos;
            this.Width = Data.SettingCh.St.View_Width;
            this.Height = Data.SettingCh.St.View_Height;
            if (Data.SettingCh.St.ViewMaximized)
            {
                WindowState = WindowState.Maximized;
            }

            if (Data.SettingCh.St.UseLocalModule == true)
            {
                UseLocalM.IsChecked = true;
            }
            else
            {
                UseLocalM.IsChecked = false;
            }
            if (Data.SettingCh.St.UseTopMenu == true)
            {
                UseTopMenu.IsChecked = true;
            }
            else
            {
                UseTopMenu.IsChecked = false;
            }
            if (Data.SettingCh.St.UseBouthModule == true)
            {
                UseBouthM.IsChecked = true;
            }
            else
            {
                UseBouthM.IsChecked = false;
            }
            if (Data.SettingCh.St.UseTopModule == true)
            {
                UseTopM.IsChecked = true;
            }
            else
            {
                UseTopM.IsChecked = false;
            }
            if (Data.SettingCh.St.UseBottomhModule == true)
            {
                UseBottomM.IsChecked = true;
            }
            else
            {
                UseBottomM.IsChecked = false;
            }
            if (Data.SettingCh.St.UseSound == true)
            {
                UseSound.IsChecked = false;
            }
            else
            {
                UseSound.IsChecked = true;
            }
            // Hot keys send
            if (Data.SettingCh.St.UseHotEnter)
            {
                UseHotEnter.IsChecked = true;
            }
            else
            {
                UseHotEnter.IsChecked = false;
            }
            if (Data.SettingCh.St.UseHotEnterCtrl)
            {
                UseHotEnterCtrl.IsChecked = true;
            }
            else
            {
                UseHotEnterCtrl.IsChecked = false;
            }
            if (Data.SettingCh.St.UseHotEnterShift)
            {
                UseHotEnterShift.IsChecked = true;
            }
            else
            {
                UseHotEnterShift.IsChecked = false;
            }

        }


        public ViewWin()
        {
            InitializeComponent();
            ReadSetting();

            if (Data.MainWin.MainGrid.RowDefinitions[0].ActualHeight > 0)
            {
                UseTopMenu.IsChecked = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WriteSetting();
        }



        private void UseBouthM_Checked(object sender, RoutedEventArgs e)
        {
            UseBottomM.IsChecked = false;
            UseTopM.IsChecked = false;
        }

        private void UseTopM_Checked(object sender, RoutedEventArgs e)
        {
            UseBouthM.IsChecked = false;
            UseBottomM.IsChecked = false;
        }

        private void UseBottomM_Checked(object sender, RoutedEventArgs e)
        {
            UseBouthM.IsChecked = false;
            UseTopM.IsChecked = false;
        }

        private void UseLocalM_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)UseLocalM.IsChecked)
            {
                Data.MainWin.MainGrid.RowDefinitions[2].Height = new GridLength(30, GridUnitType.Pixel);
            }
            else
            {
                Data.MainWin.MainGrid.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void UseTopMenu_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)UseTopMenu.IsChecked)
            {
                Data.MainWin.MainGrid.RowDefinitions[0].Height = new GridLength(Data.SettingCh.St.MenuGridHeight, GridUnitType.Pixel);
            }
            else
            {
                Data.MainWin.MainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void UseBouthM_Unchecked(object sender, RoutedEventArgs e)
        {
            if(UseTopM.IsChecked == true || UseBottomM.IsChecked == true)
            UseBouthM.IsChecked = false;
            else
                UseBouthM.IsChecked = true;
        }

        private void UseTopM_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UseBouthM.IsChecked == true || UseBottomM.IsChecked == true)
                UseTopM.IsChecked = false;
            else
                UseTopM.IsChecked = true;
        }

        private void UseBottomM_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UseBouthM.IsChecked == true || UseTopM.IsChecked == true)
                UseBottomM.IsChecked = false;
            else
                UseBottomM.IsChecked = true;
        }

        private void UseHotEnter_Checked(object sender, RoutedEventArgs e)
        {
            UseHotEnterCtrl.IsChecked = false;
            UseHotEnterShift.IsChecked = false;
        }

        private void UseHotEnterCtrl_Checked(object sender, RoutedEventArgs e)
        {
            UseHotEnter.IsChecked = false;
            UseHotEnterShift.IsChecked = false;
        }

        private void UseHotEnterShift_Checked(object sender, RoutedEventArgs e)
        {
            UseHotEnter.IsChecked = false;
            UseHotEnterCtrl.IsChecked = false;
        }

        private void UseHotEnter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UseHotEnterCtrl.IsChecked == true || UseHotEnterShift.IsChecked == true)
                UseHotEnter.IsChecked = false;
            else
                UseHotEnter.IsChecked = true;
        }

        private void UseHotEnterCtrl_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UseHotEnter.IsChecked == true || UseHotEnterShift.IsChecked == true)
                UseHotEnterCtrl.IsChecked = false;
            else
                UseHotEnterCtrl.IsChecked = true;
        }

        private void UseHotEnterShift_Unchecked(object sender, RoutedEventArgs e)
        {
            if (UseHotEnter.IsChecked == true || UseHotEnterCtrl.IsChecked == true)
                UseHotEnterShift.IsChecked = false;
            else
                UseHotEnterShift.IsChecked = true;
        }


    }
}
