using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using no.miles.at.Backend.Api;
using no.miles.at.Backend.Api.Utilities;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Infrastructure.Configuration;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace no.miles.at.Backend.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = ConfigManager.GetConfigUsingDefaultConfigFile();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Helpers.Initialize(config.CompanyId, config.StatusEndpointUrl);
            var container = UnityConfig.RegisterComponents(config);

            var identityResolver = container.Resolve<IResolveUserIdentity>();
            var commandSender = container.Resolve<ICommandSender>();
            ConfigureAuth(app, config, identityResolver, commandSender);
        }
    }
}
