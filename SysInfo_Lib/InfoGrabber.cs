using System;
using System.IO;
using System.Linq;

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
        private string GetDiskInfo(bool verbose)
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
                        result += LineFormat("Drive " + drive.Name.Replace(":\\", "") + " " + (drive.VolumeLabel != "" ? "(" + drive.VolumeLabel + ")" : "") + " - " + drive.DriveType);
                        result += LineFormat("File System: " + drive.DriveFormat, 1);
                        result += LineFormat("Free: " + ToGB(drive.TotalFreeSpace) + "/" + ToGB(drive.TotalSize), 1);
                    }
                    else
                    {
                        // C: 
                        result += LineFormat(drive.Name.Replace("\\", "") + " " + ToGB(drive.TotalFreeSpace) + "/" + ToGB(drive.TotalSize), 1);
                    }
                    
                }
            }
            return result;
        }
    }
}
