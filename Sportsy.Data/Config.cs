using System.IO;
using Newtonsoft.Json;

namespace Sportsy.Data
{
    public class Config
    {
        public string LastPath;
        public string ConfigPath;

        public static Config Load(string configFile)
        {
            if (!File.Exists(configFile))
                Save(new Config(), configFile);

            var text = File.ReadAllText(configFile);
            var config = JsonConvert.DeserializeObject<Config>(text);
            config.ConfigPath = configFile;
            return config;
        }

        public static void Save(Config config, string configFile)
        {
            File.WriteAllText(configFile,
                Newtonsoft.Json.JsonConvert.SerializeObject(config, Formatting.Indented));
            config.ConfigPath = configFile;
        }
        
    }
}
