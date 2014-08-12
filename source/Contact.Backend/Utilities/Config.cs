using System.Configuration;

namespace Contact.Backend.Utilities
{
    public class Config
    {
        public static readonly string StatusEndpoint = ConfigurationManager.AppSettings["statusEndpoint"];

        public static readonly bool UseMockCommandHandler = ConfigurationManager.AppSettings["useMockCommandHandler"] ==
                                                            "true";

        public static class Rabbit
        {
            public static string Host
            {
                get { return ConfigurationManager.AppSettings["rabbitHost"]; }
            }

            public static string Password
            {
                get { return ConfigurationManager.AppSettings["rabbitPassword"]; }
            }

            public static string Username
            {
                get { return ConfigurationManager.AppSettings["rabbitUsername"]; }
            }

            public static string ExchangeName
            {
                get { return ConfigurationManager.AppSettings["rabbitExchangeName"]; }
            }

            public static bool UseSsl
            {
                get { return ConfigurationManager.AppSettings["rabbitUseSSL"] == "true"; }
            }
        }

    }
}