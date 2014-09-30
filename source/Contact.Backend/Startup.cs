using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Helpers.Initialize(config.CompanyId, config.StatusEndpointUrl);
            UnityConfig.RegisterComponents(config);
            MapperConfig.Configure();
            ConfigureAuth(app, config);
        }
    }
}
