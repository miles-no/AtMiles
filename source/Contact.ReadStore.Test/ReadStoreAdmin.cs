using System;
using Contact.Infrastructure;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Contact.ReadStore.UserStore;
using Raven.Client;
using Raven.Client.Indexes;

namespace Contact.ReadStore
{
    public class ReadStoreAdmin
    {
        readonly ReadModelHandler handlers = new ReadModelHandler();
        public void PrepareHandlers(IDocumentStore store)
        {
            IndexCreation.CreateIndexes(GetType().Assembly, store);
            new EmployeeSearchStore(store).PrepareHandler(handlers);
            new CommandStatusStore(store).PrepareHandler(handlers);
            new UserLookupStore(new UserLookupEngine(store),store).PrepareHandler(handlers);

        }

        public void StartListening()
        {
            var demo = new EventStoreDispatcher(ReadStoreConstants.Host, ReadStoreConstants.Username, ReadStoreConstants.Password, handlers, new ConsoleLogger(), () => { }, null);
            demo.Start();
            Console.WriteLine("Listening...");
            
        }
    }
}