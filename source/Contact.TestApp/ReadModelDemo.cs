using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Contact.Domain;
using Contact.Infrastructure;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contact.TestApp
{
    public class ReadModelDemo : LongRunningProcess
    {
        private EventStoreCatchUpSubscription _subscription;
        private IEventStoreConnection _eventStoreConnection;
        private readonly string _eventStoreServer;
        private readonly string _eventStoreUsername;
        private readonly string _eventStorePassword;
        private readonly IEventPublisher _publisher;

        public ReadModelDemo(string eventStoreServer, string eventStoreUsername, string eventStorePassword, IEventPublisher publisher, ILog log)
            : base(log)
        {
            _eventStoreServer = eventStoreServer;
            _eventStoreUsername = eventStoreUsername;
            _eventStorePassword = eventStorePassword;
            _publisher = publisher;
        }

        public ReadModelDemo(ILog log) : base(log)
        {

        }

        protected override void Initialize()
        {
            var settings = ConnectionSettings.Create().SetHeartbeatInterval(TimeSpan.FromSeconds(2));
            var endPoint = GetIpEndPoint(_eventStoreServer);
            _eventStoreConnection = EventStoreConnection.Create(settings, endPoint);
            _eventStoreConnection.Connect();
        }

        protected override void Run()
        {
            RecoverSubscription();
            while (IsRunning)
            {
                Thread.Sleep(5000);

            }
            StopSubscription();
        }

        private void RecoverSubscription()
        {
            if (IsRunning)
            {
                try
                {
                    var credentials = new UserCredentials(_eventStoreUsername, _eventStorePassword);
                    var currentPosition = GetLatestPosition();
                    _subscription = _eventStoreConnection.SubscribeToAllFrom(currentPosition, false, EventHandle,
                        LiveProcessingStarted, DroppedHandle, credentials,10);
                }
                catch (Exception error)
                {
                    Log.Warn("Not able to start subscription to EventStore.", error);
                }
            }
        }

        private void StopSubscription()
        {
            if (_subscription != null)
            {
                try
                {
                    _subscription.Stop(TimeSpan.FromSeconds(10));
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                { } //Do nothing if error during cleanup
            }
        }

        private void EventHandle(EventStoreCatchUpSubscription arg1, ResolvedEvent resolvedEvent)
        {
            Event domainEvent;
            if (TryDeserializeAggregateEvent(resolvedEvent, out domainEvent))
            {
                try
                {
                    _publisher.Publish(domainEvent);
                }
                catch { }  //TODO: Log error here
            }
        }

        private void DroppedHandle(EventStoreCatchUpSubscription arg1, SubscriptionDropReason dropReason, Exception exception)
        {
            
            if (dropReason == SubscriptionDropReason.UserInitiated) return;

            if (exception != null)
            {
                Log.Warn("Connection to EventStore dropped. Reason: " + dropReason.ToString(), exception);
            }
            else
            {
                Log.Warn("Connection to EventStore dropped. Reason: " + dropReason.ToString());
            }
            Thread.Sleep(10000);
            RecoverSubscription();
        }

        private Position? GetLatestPosition()
        {
            return null;
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription arg1)
        {
            Log.Info("TimeLine starting live processing.");
        }

        private static System.Net.IPEndPoint GetIpEndPoint(string serverName, int portNumber = 1113)
        {
            var addresses = System.Net.Dns.GetHostAddresses(serverName);
            if (addresses.Length == 0) throw new Exception(serverName);
            return new System.Net.IPEndPoint(addresses[0], portNumber);
        }

        private static bool TryDeserializeAggregateEvent(ResolvedEvent rawEvent, out Event deserializedEvent)
        {
            deserializedEvent = null;

            if (rawEvent.OriginalEvent.EventType.StartsWith("$") || rawEvent.OriginalEvent.EventStreamId.StartsWith("$"))
                return false;

            IDictionary<string, JToken> metadata;
            try
            {
                metadata = JObject.Parse(Encoding.UTF8.GetString(rawEvent.OriginalEvent.Metadata));
            }
            catch (JsonReaderException)
            {
                return false;
            }

            if (!metadata.ContainsKey(Constants.EventStoreEventClrTypeHeader))
                return false;

            Type deserializeTo;
            try
            {
                deserializeTo = Type.GetType((string)metadata[Constants.EventStoreEventClrTypeHeader], true);
            }
            catch (Exception) //TODO: Log error here
            {
                return false;
            }

            try
            {
                var jsonString = Encoding.UTF8.GetString(rawEvent.OriginalEvent.Data);
                deserializedEvent = JsonConvert.DeserializeObject(jsonString, deserializeTo) as Event;
                return deserializedEvent != null;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
