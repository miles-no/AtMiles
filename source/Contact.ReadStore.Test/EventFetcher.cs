using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Contact.ReadStore.Test
{
    public class EventFetcher
    {
        public async Task Subscribe(Func<ResolvedEvent,Task> whenResolved,  string[] justTheseEvents = null)
        {

            var connectionSettingsBuilder = ConnectionSettings.Create();
            connectionSettingsBuilder.SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
            connectionSettingsBuilder.KeepReconnecting();
            connectionSettingsBuilder.KeepRetrying();
            connectionSettingsBuilder.SetHeartbeatTimeout(TimeSpan.FromSeconds(1));

            var connection = EventStoreConnection.Create(connectionSettingsBuilder, new IPEndPoint(new IPAddress(new byte[] { 191, 239, 209, 249 }), 1113));

            connection.Connect();

            connection.SubscribeToAllFrom(null, false, async (subscription, @event) =>
                {
                    if (justTheseEvents != null && justTheseEvents.Any(a => a == @event.Event.EventType) == false)
                    {
                        return;
                    }

                    await whenResolved(@event);
                },
                subscription => { },
                (subscription, reason, ex) => Console.WriteLine("Dropped. Reason:" + reason)
                , null, 10);

        }
    }
}
