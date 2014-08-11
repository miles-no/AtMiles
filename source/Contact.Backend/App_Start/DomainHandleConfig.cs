using System.Runtime.CompilerServices;
using Contact.Backend.DomainHandlers;
using Contact.Backend.Infrastructure;
using Microsoft.Practices.Unity;

namespace Contact.Backend
{
    public class MediatorConfig
    {
        public static IMediator Create(IUnityContainer container)
        {
            var mediator = new Mediator();

            Handlers.CreateHandlers(mediator, container);

            return mediator;
        }
    }
}