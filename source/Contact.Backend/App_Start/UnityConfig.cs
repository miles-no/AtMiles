using Contact.Backend.Infrastructure;
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
            container.RegisterType<IMediator, Mediator>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}