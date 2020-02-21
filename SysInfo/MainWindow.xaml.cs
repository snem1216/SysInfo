using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
using System.Management;
using System.IO;

namespace SysInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ScreenWrite(Environment.WorkingSet.ToString());
            WriteUserInfo();
            WriteHostName();
            WriteOSInfo();
            ScreenWrite("RAM: " + GetRAM());
            WriteDiskInfo();
            WriteNetworkInfo();
            try
            {
                tb.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ff000");
            }
            catch
            {
                Console.WriteLine("Failed to load text color from config");
            }
        }

        public void ScreenWrite(string x, int tabs = 0)
        {
            tb.Text += String.Concat(Enumerable.Repeat("\t", tabs)) + x + "\n";
        }
        private static string ToGB(long s)
        {
            return String.Format("{0:0.00}", (s / Math.Pow(1024, 3))) + " GB";
        }
        private void WriteUserInfo()
        {
            ScreenWrite("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
        }
        private void WriteDiskInfo()
        {
            DriveInfo[] Drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in Drives)
            {
                if (drive.IsReady == true)
                {
                    ScreenWrite("Drive " + drive.Name.Replace(":\\","") + " " + (drive.VolumeLabel != "" ? "(" + drive.VolumeLabel + ")" : "") + " - " + drive.DriveType);
                    ScreenWrite("File System: " + drive.DriveFormat,1);
                    ScreenWrite("Free: " + ToGB(drive.TotalFreeSpace) + "/" + ToGB(drive.TotalSize), 1);
                }
            }
        }

        private string GetOsName()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();

            return name != null ? name.ToString() : "Unknown";
        }

        private string GetRAM()
        {
            string totalRAM;
            var ramsize = (from x in new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("TotalPhysicalMemory")).FirstOrDefault();
            totalRAM = String.Format("{0:.##} GB", ((Int64.Parse(ramsize.ToString())) / Math.Pow(1024, 3)));
            return totalRAM != null ? totalRAM.ToString() : "Unknown";
        }

        private void WriteHostName()
        {
            string hostName = Dns.GetHostName();
            ScreenWrite("Hostname: " + hostName);
        }
        private void WriteNetworkInfo()
        {
            // Network Info
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    ScreenWrite(ni.Name);
                    ScreenWrite("Physical: " + ni.GetPhysicalAddress(), 1);
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ScreenWrite("IP: " + ip.Address.ToString(), 1);
                            ScreenWrite("Subnet Mask: " + ip.IPv4Mask, 1);
                        }
                    }
                }
            }
        }
        private void WriteOSInfo()
        {
            // OS Version Info
            var os = Environment.OSVersion;
            string BuildVersion = (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ReleaseId");
            ScreenWrite("OS: " + GetOsName());
            ScreenWrite("OS Build: " + BuildVersion);
            ScreenWrite("Service Pack: " + (os.ServicePack != "" ? os.ServicePack : "N/A"));
        }

        // Make this display in the bottom right-hand corner
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = SystemParameters.WorkArea.Right - (this.Width + 5);
            this.Top = SystemParameters.WorkArea.Bottom - (this.Height + 5);
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
    }
}
