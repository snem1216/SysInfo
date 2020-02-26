using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SysInfo
{
    public class CONFIG_LOADER
    {
        // Default configuration values
        private static Dictionary<string, string> DefaultConfig = new Dictionary<string, string>
        {
            { "TextColor","#fff" },
            { "TextSize", "14" },
            { "TextFont", "Consolas" },
            { "VerticalLocation", "Bottom" },
            { "HorizontalLocation", "Right" },
            { "AlwaysOnTop", "False" },
            { "ShowUsername", "True" },
            { "ShowHostname", "True" },
            { "ShowOS", "True" },
            { "ShowRAM", "True" },
            { "DriveInfo", "Verbose" },
            { "NetInfo", "Compact" }
        };
        private static void WriteDefaultConfFile(string path)
        {
            string contents = "";
            foreach(string key in DefaultConfig.Keys)
            {
                contents += key + "=" + DefaultConfig[key] + "\n";
            }
            System.IO.File.WriteAllText(path, contents);
        }
        public static Dictionary<string, string> GetConf(string ConfigFilePath = "sysinfo-settings.conf")
        {
            // Use the default configuration, and modify it as settings are passed in
            // This ensures the configuration is passed through, even if the configuration file is used as an override (partial)
            Dictionary<string, string> ReturnDict = DefaultConfig;
            if (File.Exists(ConfigFilePath))
            {
                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitLine = line.Split('=');
                        if((splitLine.Length == 2) && !line.StartsWith("#"))
                        {
                            if(ReturnDict.Keys.Contains(splitLine[0]))
                            {
                                ReturnDict[splitLine[0]] = splitLine[1];
                            }
                            else
                            {
                                ReturnDict.Add(splitLine[0], splitLine[1]);
                            }
                        }
                    }
                }
                return ReturnDict;
            }
            else
            {
                // Use default configuration
                Console.WriteLine("Could not find " + ConfigFilePath);
                try
                {
                    WriteDefaultConfFile(ConfigFilePath);
                }
                catch
                {
                    Console.WriteLine("Failed to write default config");
                }
                return DefaultConfig;
            }
        }

            
    }
        
}
