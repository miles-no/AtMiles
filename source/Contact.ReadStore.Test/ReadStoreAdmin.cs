using System;
using Contact.Infrastructure;
using Contact.ReadStore.Test.SearchStore;
using Contact.ReadStore.Test.SessionStore;
using Contact.ReadStore.Test.UserStore;
using Raven.Client;

namespace Contact.ReadStore.Test
{
    public class ReadStoreAdmin
    {
        readonly ReadModelHandler handlers = new ReadModelHandler();
        public void PrepareHandlers(IDocumentStore store)
        {
            new EmployeeSearchStore(store).PrepareHandler(handlers);
            new CommandStatusStore(store).PrepareHandler(handlers);
            new UserLookupStore(new UserLookupEngine(store),store).PrepareHandler(handlers);

        }

        public void StartListening()
        {
            var demo = new EventStoreDispatcher(ReadStoreConstants.Host, ReadStoreConstants.Username, ReadStoreConstants.Password, handlers, new ConsoleLogger(), () => { });
            demo.Start();
            Console.WriteLine("Listening...");
            
        }
    }
}