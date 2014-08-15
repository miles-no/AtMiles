using System;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Contact.TestApp
{
    public class ReadModelTesting
    {
        public void TestSubscription()
        {
            Console.WriteLine("Wait for it... Takes like half a minute...");
            var connectionSettingsBuilder = ConnectionSettings.Create();
            connectionSettingsBuilder.SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
            connectionSettingsBuilder.KeepReconnecting();
            connectionSettingsBuilder.KeepRetrying();
            connectionSettingsBuilder.SetHeartbeatTimeout(TimeSpan.FromSeconds(40));
            connectionSettingsBuilder.SetOperationTimeoutTo(TimeSpan.FromSeconds(120));
            connectionSettingsBuilder.SetTimeoutCheckPeriodTo(TimeSpan.FromMinutes(1));
            
            var connection = EventStoreConnection.Create(connectionSettingsBuilder,new IPEndPoint(new IPAddress(new byte[]{191,239,209,249}),1113 ));
            
            connection.Connect();
        
            connection.SubscribeToAllFrom(null, false,
                (subscription, @event) => Console.WriteLine(@event.Event.EventType),
                subscription =>{},
                (subscription, reason, arg3) => {},
                new UserCredentials("admin", "changeit"), 10);
        }

    }
}