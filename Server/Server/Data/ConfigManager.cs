using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string? dataPath;
        public string? connectionString;
    }
    public class ConfigManager
    {
        public static ServerConfig? Config { get; private set; }
        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json");
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
}
