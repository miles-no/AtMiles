using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Contact.Domain;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contact.Infrastructure
{
    public class EventStoreDispatcher
    {
        private readonly IPersistGetEventStorePosition _positionCheckpoint;
        private readonly IEventPublisher _next;
        private readonly IEventStoreConnection _connection;
        private readonly Action<EventStoreCatchUpSubscription> _onLiveProcessingStarted;

        private readonly System.Net.IPEndPoint _endPoint;
        private readonly UserCredentials _credentials;

        private EventStoreCatchUpSubscription _subscription;

        public EventStoreDispatcher(string serverName, string username, string password, IEventPublisher publisher, IPersistGetEventStorePosition positionCheckpoint = null,
                Action<EventStoreCatchUpSubscription> onLiveProcessingStarted = null)
        {
            if (publisher == null)
                throw new ArgumentNullException("publisher");

            _endPoint = GetIpEndPoint(serverName);
            _credentials = new UserCredentials(username, password);

            _positionCheckpoint = positionCheckpoint;
            _next = publisher;
            _onLiveProcessingStarted = onLiveProcessingStarted;

            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(_credentials)
                .KeepReconnecting()
                .KeepRetrying();

            _connection = EventStoreConnection.Create(settings, _endPoint);
            _connection.Connect();
        }

        private void SetUpSubscription()
        {
            Position? fromPosition = null;
            if (_positionCheckpoint != null)
            {
                fromPosition = _positionCheckpoint.GetLastProcessedPosition();
            }
            
            _subscription = _connection.SubscribeToAllFrom(null, false, EventAppeared, null, null, _credentials);

            //_subscription = _connection.SubscribeToAllFrom(fromPosition, false, EventAppeared, _onLiveProcessingStarted, Dropped,_credentials);
        }

        private static System.Net.IPEndPoint GetIpEndPoint(string serverName, int portNumber = 1113)
        {
            var addresses = System.Net.Dns.GetHostAddresses(serverName);
            if (addresses.Length == 0) throw new Exception(serverName);
            return new System.Net.IPEndPoint(addresses[0], portNumber);
        }

        private void Dropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason dropReason, Exception exception)
        {
            if (dropReason == SubscriptionDropReason.ProcessingQueueOverflow)
            {
                WaitAndRetry();
                return;
            }

            if (dropReason == SubscriptionDropReason.UserInitiated)
                return;

            if (SubscriptionDropMayBeRecoverable(dropReason))
                SetUpSubscription();
        }

        private void WaitAndRetry()
        {
            Thread.Sleep(5000);
            SetUpSubscription();
        }

        private bool SubscriptionDropMayBeRecoverable(SubscriptionDropReason dropReason)
        {
            return dropReason == SubscriptionDropReason.Unknown || dropReason == SubscriptionDropReason.SubscribingError ||
            dropReason == SubscriptionDropReason.ServerError || dropReason == SubscriptionDropReason.ConnectionClosed;
        }

        private void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            Event domainEvent;
            if (TryDeserializeAggregateEvent(resolvedEvent, out domainEvent))
            {
                try
                {
                    _next.Publish(domainEvent);
                }
                catch { }  //TODO: Log error here
            }

            if (resolvedEvent.OriginalPosition.HasValue)
                _positionCheckpoint.PersistLastPositionProcessed(resolvedEvent.OriginalPosition);
        }

        public void Start()
        {
            SetUpSubscription();
        }

        public void Stop(TimeSpan timeout)
        {
            _subscription.Stop(timeout);
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
