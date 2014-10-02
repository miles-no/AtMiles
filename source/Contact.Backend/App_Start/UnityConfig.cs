using Contact.Backend.Controllers;
using Contact.Domain;
using Contact.Infrastructure;
using Contact.ReadStore;
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
        public static IUnityContainer RegisterComponents(Contact.Infrastructure.Configuration.Config config)
        {
            var container = new UnityContainer();

            container.RegisterType<AdminController>();
            container.RegisterType<StatusController>();
            container.RegisterType<TestController>();
            container.RegisterType<HomeController>();
            container.RegisterType<SearchController>();
            container.RegisterInstance(typeof(IDocumentStore), RavenDocumentStore.CreateStore(config.RavenDbUrl));
            container.RegisterInstance(MediatorConfig.Create(container));
            container.RegisterType<EmployeeSearchEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<CommandStatusEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<CommandStatusEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IResolveUserIdentity), typeof(UserLookupEngine), new ContainerControlledLifetimeManager());

            container.RegisterInstance(typeof(ICommandSender),
                new RabbitMqCommandSender(
                    config.RabbitMqHost,
                    config.RabbitMqUsername,
                    config.RabbitMqPassword,
                    config.RabbitMqCommandExchangeName,
                    config.RabbitMqUseSsl));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            return container;
        }
    }
}