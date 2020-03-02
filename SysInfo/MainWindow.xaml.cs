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
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

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
            // Do not run multiple instances
            if(System.Diagnostics.Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length != 1)
            {
                this.Close();
            }

            InitializeComponent();
            //ScreenWrite(Environment.WorkingSet.ToString());
            if(config["RefreshInterval"] != "0")
            {
                int refreshRate;
                try
                {
                    refreshRate = int.Parse(config["RefreshInterval"]);
                }
                catch
                {
                    refreshRate = int.Parse(CONFIG_LOADER.GetDefaultConfig()["RefreshInterval"]);
                }
                refreshRate *= 1000;
                var Refresh = new Task(() => IncRefresh(refreshRate),
                    TaskCreationOptions.LongRunning);
                Refresh.Start();
            }
            WriteScreen(tb);
            ConfigAppearance();
        }

        private void IncRefresh(int refreshRate)
        {
            while(true)
            {
                Thread.Sleep(refreshRate);
                WriteScreen(tb);
            }
           
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
        private void WriteScreen(TextBlock target)
        {
            string ScreenData = "";
            if (config["RefreshInterval"] != "0") ScreenData += "Stats Updated: \n\t" + DateTime.Now + "\n";
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
            this.Dispatcher.Invoke(() => {
                target.Text = ScreenData;
            });

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
