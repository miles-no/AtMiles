using Contact.Backend.Controllers;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Infrastructure;
using Contact.ReadStore.Test;
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
            container.RegisterType<SearchController>();
            container.RegisterInstance(MediatorConfig.Create(container));
            container.RegisterInstance(new SearchEngine());

            //TODO: Get a proper IdentityResolver
            container.RegisterInstance<IResolveUserIdentity>(new DummyAndHardCodedIdentityReseolver());
            

            if (Config.UseMockCommandHandler)
            {
                container.RegisterType<ICommandSender, CommandSenderMock>();
            }
            else
            {
                container.RegisterInstance(typeof(ICommandSender), 
                    new RabbitMqCommandSender(
                        Config.Rabbit.Host, 
                        Config.Rabbit.Username, 
                        Config.Rabbit.Password, 
                        Config.Rabbit.ExchangeName, 
                        Config.Rabbit.UseSsl));
            }
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}