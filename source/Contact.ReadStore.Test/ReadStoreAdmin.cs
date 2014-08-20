using System;
using Contact.Infrastructure;
using Contact.ReadStore.Test.SearchStore;
using Contact.ReadStore.Test.SessionStore;

namespace Contact.ReadStore.Test
{
    public class ReadStoreAdmin
    {
        readonly ReadModelHandler handlers = new ReadModelHandler();
        public void PrepareHandlers()
        {
            new EmployeeSearchStore().AddFillAndPrepareHandler(handlers);
            new CommandStatusStore().AddCommandStatusStoreUpdatedHandler(handlers);

        }

        public void StartListening()
        {
            var demo = new EventStoreDispatcher(Constants.host, Constants.username, Constants.password, handlers, new ConsoleLogger(), () => { });
            demo.Start();
            Console.WriteLine("Listening...");
            
        }
    }
}