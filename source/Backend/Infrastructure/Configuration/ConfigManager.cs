using System.IO;
using YamlDotNet.Serialization;

namespace no.miles.at.Backend.Infrastructure.Configuration
{
    public class ConfigManager
    {
        public static Config GetConfig(string configFilename)
        {
            Config config = null;
            using (var reader = File.OpenText(configFilename))
            {
                var deserializer = new Deserializer();
                config = deserializer.Deserialize<Config>(reader);
            }
            return config;
        }
    }
}
