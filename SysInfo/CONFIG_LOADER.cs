using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysInfo
{
    class CONFIG_LOADER
    {
        public Dictionary<string, string> LoadSettings()
        {
            string ConfigFilePath = "sysinfo-config.csv";
            Dictionary<string, string> ReturnDict = new Dictionary<string, string>();
            if(File.Exists(ConfigFilePath))
            using (StreamReader reader = new StreamReader(ConfigFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(',');
                    ReturnDict.Add(splitLine[0], splitLine[1]);
                }
            }
            return ReturnDict;
        }
        
    }
}
