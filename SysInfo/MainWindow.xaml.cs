using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.IO;
using SysInfo_Lib;

namespace SysInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InfoGrabber siig = new InfoGrabber();
        Dictionary<string, string> config = CONFIG_LOADER.GetConf();
        //Dictionary<string, string> config
        public MainWindow()
        {
            InitializeComponent();
            //ScreenWrite(Environment.WorkingSet.ToString());
            tb.Text = WriteScreen();
            ConfigAppearance();
        }

        // Apply aesthetic settings to the window
        private void ConfigAppearance()
        {
            // Load Foreground Color
            try
            {
                tb.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(config["TextColor"]);
            }
            catch
            {
                Console.WriteLine("Failed to load text color from config");
            }
            tb.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString(config["TextFont"]);
            tb.FontSize = Convert.ToDouble(config["TextSize"]);
            this.Topmost = (config["AlwaysOnTop"] == "True");
            int hpadding = 5;
            int vpadding = 5;
            switch (config["HorizontalLocation"])
            {
                case "Left":
                    this.Left = hpadding;
                    break;
                // Not sure why anyone would pick this, but just in case...
                case "Center":
                    this.Left = (SystemParameters.WorkArea.Right - this.Width) / 2;
                    break;
                // Default setting is "Right"
                default:
                    this.Left = SystemParameters.WorkArea.Right - (this.Width + hpadding);
                    break;
            }
            switch (config["VerticalLocation"])
            {
                case "Top":
                    this.Top = vpadding;
                    break;
                case "Center":
                    this.Top = (SystemParameters.WorkArea.Bottom - this.Height) / 2;
                    break;
                // Default setting is "Bottom"
                default:
                    this.Top = SystemParameters.WorkArea.Bottom - (this.Height + vpadding);
                    break;
            }
            
        }
        private string WriteScreen()
        {
            string ScreenData = "";
            if (config["ShowUsername"] == "True") ScreenData += siig.GetUserInfo();
            if(config["ShowHostname"] == "True") ScreenData += siig.GetHostname();
            if (config["ShowOS"] == "True") ScreenData += siig.GetOSInfo();
            if (config["ShowRAM"] == "True") ScreenData += siig.GetRAM();
            if (config["DriveInfo"] != "None")
            {
                ScreenData += siig.GetDiskInfo(config["DriveInfo"] == "Verbose");
            }
            if (config["NetInfo"] != "None")
            {
                ScreenData += siig.GetNetworkInfo(config["NetInfo"] == "Verbose");
            }
            return ScreenData;
        }


        // Disable minimization
        // Will still minimize when "Show Desktop" is triggered, but will pop back up as soon as another Window is restored/launched.
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigAppearance();
        }
    }
}
