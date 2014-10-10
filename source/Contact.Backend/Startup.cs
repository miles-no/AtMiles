using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Contact.Backend.Properties;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Infrastructure;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using Owin;

[assembly: OwinStartup(typeof(Contact.Backend.Startup))]

namespace Contact.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = Contact.Infrastructure.Configuration.ConfigManager.GetConfig(Settings.Default.ConfigFile);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Helpers.Initialize(config.CompanyId, config.StatusEndpointUrl);
            var container = UnityConfig.RegisterComponents(config);

            var identityResolver = container.Resolve<IResolveUserIdentity>();
            var commandSender = container.Resolve<ICommandSender>();
            ConfigureAuth(app, config, identityResolver, commandSender);
        }
    }
}
