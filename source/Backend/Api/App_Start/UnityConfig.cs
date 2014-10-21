using System.Web.Http;
using Microsoft.Practices.Unity;
using no.miles.at.Backend.Api.Controllers;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Infrastructure.Configuration;
using no.miles.at.Backend.ReadStore;
using no.miles.at.Backend.ReadStore.BusyTimeStore;
using no.miles.at.Backend.ReadStore.SearchStore;
using no.miles.at.Backend.ReadStore.SessionStore;
using no.miles.at.Backend.ReadStore.UserStore;
using Raven.Client;
using Unity.WebApi;

namespace no.miles.at.Backend.Api
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents(Config config)
        {
            var container = new UnityContainer();

            container.RegisterType<AdminController>();
            container.RegisterType<EmployeeController>();
            container.RegisterType<RootController>();
            container.RegisterType<SearchController>();
            container.RegisterType<StatusController>();
            container.RegisterInstance(typeof(IDocumentStore), RavenDocumentStore.CreateStore(config.RavenDbUrl));
            container.RegisterType<EmployeeSearchEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<CommandStatusEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<BusyTimeEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType<UserLookupEngine>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IResolveUserIdentity), typeof(UserLookupEngine), new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IResolveNameOfUser), typeof(UserLookupEngine), new ContainerControlledLifetimeManager());

            var logger = new EventLogger("MilesSource", "AtMilesLog");
            container.RegisterInstance(typeof(ICommandSender),
                new RabbitMqCommandSender(
                    config.RabbitMqHost,
                    config.RabbitMqUsername,
                    config.RabbitMqPassword,
                    config.RabbitMqCommandExchangeName,
                    config.RabbitMqUseSsl, logger));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            return container;
        }
    }
}