using System.Configuration;

namespace Contact.Backend.Utilities
{
    public class Config
    {
        public static readonly string StatusEndpoint = ConfigurationManager.AppSettings["statusEndpoint"];
        public static readonly bool UseMockCommandHandler = ConfigurationManager.AppSettings["useMockCommandHandler"] == "true";

        public static class Rabbit
        {
            public static readonly string Host = ConfigurationManager.AppSettings["rabbitHost"];
            public static readonly string Password = ConfigurationManager.AppSettings["rabbitPassword"];
            public static readonly string Username = ConfigurationManager.AppSettings["rabbitUsername"];
            public static readonly string ExchangeName = ConfigurationManager.AppSettings["rabbitExchangeName"];
            public static readonly bool UseSsl = ConfigurationManager.AppSettings["rabbitUseSSL"] == "true";
            
        }
        
    }
}