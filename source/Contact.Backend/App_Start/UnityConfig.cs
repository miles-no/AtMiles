using System.Configuration;
using Contact.Backend.Controllers;
using Contact.Backend.MockStore;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Infrastructure;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Contact.ReadStore.UserStore;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Raven.Client;
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
            container.RegisterInstance(typeof (IDocumentStore), RavenDocumentStore.CreateStore(ConfigurationManager.AppSettings["ravenUrl"]));
            container.RegisterInstance(MediatorConfig.Create(container));
            container.RegisterType<EmployeeSearchEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<CommandStatusEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<CommandStatusEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IResolveUserIdentity), typeof(UserLookupEngine), new ContainerControlledLifetimeManager());
            
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