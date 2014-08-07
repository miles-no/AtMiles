using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Contact.Backend.Startup))]

namespace Contact.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
