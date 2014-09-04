using Contact.Backend.Properties;
using Contact.Backend.Utilities;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Contact.Backend.Startup))]

namespace Contact.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = Contact.Infrastructure.Configuration.ConfigManager.GetConfig(Settings.Default.ConfigFile);
            Helpers.Initialize(config.CompanyId, config.StatusEndpointUrl);
            UnityConfig.RegisterComponents(config);
            MapperConfig.Configure();
            ConfigureAuth(app);
        }
    }
}
