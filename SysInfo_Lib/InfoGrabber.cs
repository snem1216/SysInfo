using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace SysInfo_Lib
{
    public class InfoGrabber
    {
        // Format for screen output
        private string LineFormat(string line, int tabs = 0)
        {
            return String.Concat(Enumerable.Repeat("\t", tabs)) + line + "\n";
        }

        // Convert byte count to GB with 2 decimal places
        private static string ToGB(long s)
        {
            return String.Format("{0:0.00}", (s / Math.Pow(1024, 3)));
        }

        private string MACFormat(string raw, char delimiter = ':')
        {
            string formatted = "";
            if(raw.Length == 12)
            {
                for(int i=0; i < 12; i += 2)
                {
                    formatted += raw[i].ToString() + raw[i + 1].ToString();
                    if(i != 10)
                    {
                        formatted += delimiter.ToString();
                    }
                }
                return formatted;
            }
            else
            {
                return raw;
            }
        }

        // Get OS Caption
        private string GetOSCaption()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();

            return name != null ? name.ToString() : "Unknown";
        }

        public string GetRAM()
        {
            string totalRAM;
            var ramsize = (from x in new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem").Get().Cast<ManagementObject>()
                           select x.GetPropertyValue("TotalPhysicalMemory")).FirstOrDefault();
            totalRAM = String.Format("RAM: {0:.##} GB", ((Int64.Parse(ramsize.ToString())) / Math.Pow(1024, 3)));
            return LineFormat(totalRAM != null ? totalRAM : "RAM: Unknown");
        }

        public string GetDiskInfo(bool verbose)
        {
            string result = "";
            DriveInfo[] Drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in Drives)
            {
                if (drive.IsReady == true)
                {
                    if(verbose)
                    {
                        // Drive C (Windows) - Fixed
                        //  File System: NTFS
                        //  Free: 86.75 GB / 309.00 GB
                        result += LineFormat("Drive " + drive.Name.Replace(":\\", "") + (drive.VolumeLabel != "" ? " (" + drive.VolumeLabel + ")" : "") + " - " + drive.DriveType);
                        result += LineFormat("File System: " + drive.DriveFormat, 1);
                        result += LineFormat("Used: " + ToGB(drive.TotalSize - drive.TotalFreeSpace) + "/" + ToGB(drive.TotalSize) + " GB", 1);
                    }
                    else
                    {
                        // C: 
                        result += LineFormat("Drives:");
                        result += LineFormat(drive.Name.Replace("\\", "") + " " + ToGB(drive.TotalSize - drive.TotalFreeSpace) + "/" + ToGB(drive.TotalSize) + " GB", 1);
                    }
                    
                }
            }
            return result;
        }

        public string GetHostname()
        {
            string hostname = Dns.GetHostName();
            return LineFormat("Hostname: " + (hostname != null ? hostname : "Unknown"));
        }

        public string GetOSInfo()
        {
            string result = "";
            // OS Version Info
            var os = Environment.OSVersion;
            string BuildVersion = (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ReleaseId");
            result += LineFormat("OS: " + GetOSCaption());
            result += LineFormat("OS Build: " + BuildVersion);
            result += LineFormat("Service Pack: " + (os.ServicePack != "" ? os.ServicePack : "N/A"));
            return result;
        }
        public string GetUserInfo()
        {
            return LineFormat("Username: " + Environment.UserDomainName + "\\" + Environment.UserName);
        }
        public string GetNetworkInfo(bool verbose)
        {
            string result = "";
            // Network Info
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    result += LineFormat(ni.Name);
                    result += LineFormat("MAC: " + MACFormat(ni.GetPhysicalAddress().ToString()), 1);
                    if(verbose) result += LineFormat("Def. Gateway: " + (ni.GetIPProperties().GatewayAddresses.Count != 0 ? ni.GetIPProperties().GatewayAddresses[0].Address.ToString() : "N/A"), 1);
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            result += LineFormat("IP: " + ip.Address.ToString(), 1);
                            if(verbose) result += LineFormat("Subnet Mask: " + ip.IPv4Mask, 1);
                        }
                    }
                }
            }
            return result;
        }
    }
}
