using System.Configuration;
using System.IO;
using no.miles.at.Backend.Domain;
using YamlDotNet.Serialization;

namespace no.miles.at.Backend.Infrastructure.Configuration
{
    public static class ConfigManager
    {
        private static Config GetConfig(string configFilename)
        {
            Config config;
            using (var reader = File.OpenText(configFilename))
            {
                var deserializer = new Deserializer();
                config = deserializer.Deserialize<Config>(reader);
            }
            return config;
        }

        public static Config GetConfigUsingDefaultConfigFile()
        {
            var filename = ReadSetting(Constants.ConfigFilenameSetting);
            return GetConfig(filename);
        }

        private static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "Not Found";
        }
    }
}
