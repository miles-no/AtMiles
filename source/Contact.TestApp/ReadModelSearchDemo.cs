using System;
using System.Net;
using System.Threading.Tasks;
using Contact.Domain.Events.Employee;
using Contact.Infrastructure;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Contact.TestApp
{
    public class ReadModelFillFromEventsDemo
    {
        //public void TestSubscription()
        //{
            
        //    var connectionSettingsBuilder = ConnectionSettings.Create();
        //    connectionSettingsBuilder.SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
        //    connectionSettingsBuilder.KeepReconnecting();
        //    connectionSettingsBuilder.KeepRetrying();
        //    connectionSettingsBuilder.SetHeartbeatTimeout(TimeSpan.FromSeconds(1));
            
        //    var connection = EventStoreConnection.Create(connectionSettingsBuilder,new IPEndPoint(new IPAddress(new byte[]{191,239,209,249}),1113 ));
            
        //    connection.Connect();
        
        //    connection.SubscribeToAllFrom(null, false,
        //        (subscription, @event) => Console.WriteLine(@event.Event.EventType),
        //        subscription =>{},
        //        (subscription, reason, ex) => Console.WriteLine("Dropped. Reason:" + reason)
        //        ,null, 10);
        //}
      

        public void Fill(string eventStoreServer, string eventStoreUsername, string eventStorePassword)
        {
            
            var handler = new ReadModelHandler();
            handler.RegisterHandler<EmployeeCreated>(FillRaven);
            var demo = new EventStoreDispatcher(eventStoreServer, eventStoreUsername, eventStorePassword, handler, new ConsoleLogger(), () => {});
        
            demo.Start();

        }

        private void FillRaven(EmployeeCreated emp)
        {
             
        }

      
    }
}