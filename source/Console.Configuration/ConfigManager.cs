using System.IO;
using Newtonsoft.Json;

namespace Contact.Configuration
{
    public class ConfigManager
    {
        public static Config GetConfig(string configFilename)
        {
            if (!File.Exists(configFilename)) throw new FileNotFoundException();

            var raw = File.ReadAllText(configFilename);
            var config = JsonConvert.DeserializeObject<Config>(raw);

            return config;
        }
    }
}
