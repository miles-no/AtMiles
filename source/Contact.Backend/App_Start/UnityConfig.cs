using Contact.Backend.Controllers;
using Contact.Backend.Infrastructure;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Infrastructure;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Contact.Backend
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
         
            container.RegisterType<AdminController>();
            container.RegisterType<OfficeAdminController>();
            container.RegisterType<StatusController>();
            container.RegisterType<AccountController>();
            container.RegisterType<TestController>();
            container.RegisterType<HomeController>();
            container.RegisterInstance(MediatorConfig.Create(container));

            if (Config.UseMockCommandHandler)
            {
                container.RegisterType<ICommandSender, CommandSenderMock>();
            }
            else
            {
                container.RegisterType<ICommandSender, RabbitMqCommandSender>();
            }
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}