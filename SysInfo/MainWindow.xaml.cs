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
            //WriteNetworkInfo();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = 0;
            
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
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    ScreenWrite("Drive " + d.Name.Replace(":\\","") + " " + (d.VolumeLabel != "" ? "(" + d.VolumeLabel + ")" : "") + " - " + d.DriveType);
                    //ScreenWrite("Drive type: " + , 1);
                    ScreenWrite("Volume label: " + d.VolumeLabel,1);
                    ScreenWrite("File System: " + d.DriveFormat,1);
                    ScreenWrite("Free: " + ToGB(d.TotalFreeSpace) + "/" + ToGB(d.TotalSize), 1);
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
            long ram;
            var ramsize = (from x in new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("TotalPhysicalMemory")).FirstOrDefault();
            //ramsize = ((int)ramsize / (Math.Pow(1024,3)));
            string rs = ramsize.ToString();
            //string rs = "1234";
            //string r = "";

            ram = (Int64.Parse(rs));
            //int ram = Int32.Parse(ramsize.ToString());
            ramsize = String.Format("{0:.##} GB", (ram / Math.Pow(1024, 3)));
            return ramsize != null ? ramsize.ToString() : "Unknown";
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
    }
}
