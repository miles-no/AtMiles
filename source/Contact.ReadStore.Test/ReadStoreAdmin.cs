using System;
using Contact.Infrastructure;
using Contact.ReadStore.Test.SearchStore;
using Contact.ReadStore.Test.SessionStore;
using Contact.ReadStore.Test.UserStore;

namespace Contact.ReadStore.Test
{
    public class ReadStoreAdmin
    {
        readonly ReadModelHandler handlers = new ReadModelHandler();
        public void PrepareHandlers()
        {
            new EmployeeSearchStore().PrepareHandler(handlers);
            new CommandStatusStore().PrepareHandler(handlers);
            new UserLookupStore().PrepareHandler(handlers);

        }

        public void StartListening()
        {
            var demo = new EventStoreDispatcher(ReadStoreConstants.Host, ReadStoreConstants.Username, ReadStoreConstants.Password, handlers, new ConsoleLogger(), () => { });
            demo.Start();
            Console.WriteLine("Listening...");
            
        }
    }
}