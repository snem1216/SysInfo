using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SysInfo
{
    class CONFIG_LOADER
    {
        // Default configuration values
        private Dictionary<string, string> DefaultConfig = new Dictionary<string, string>
        {
            { "TextColor","#fff" },
            { "TextSize","12" },
            { "TextFont","Consolas" },
            { "Shadow","True" },
            { "VerticalLocation","Bottom" },
            { "HorizontalLocation","Right" }
        };
        public Dictionary<string, string> GetConf()
        {
            string ConfigFilePath = "sysinfo-settings.conf";

            if (File.Exists(ConfigFilePath))
            {
                Dictionary<string, string> ReturnDict = new Dictionary<string, string>();
                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitLine = line.Split('=');
                        ReturnDict.Add(splitLine[0], splitLine[1]);
                    }
                }
                return ReturnDict;
            }
            else
            {
                // Use default configuration 
                return DefaultConfig;
            }
        }

            
    }
        
}
