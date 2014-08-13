using Contact.Domain;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contact.Infrastructure
{
    public class EventStoreRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly System.Net.IPEndPoint _endPoint;
        private readonly IEventPublisher[] _publishers;
        private readonly UserCredentials _credentials;

        public EventStoreRepository(string serverName, IEventPublisher[] publishers, string username, string password)
        {
            _publishers = publishers;
            _endPoint = GetIpEndPoint(serverName);
            _credentials = new UserCredentials(username, password);
        }

        public EventStoreRepository(string serverName, int portNumber, IEventPublisher[] publishers)
        {
            _endPoint = GetIpEndPoint(serverName, portNumber);
            _publishers = publishers;
        }

        public void Save(T aggregate, int expectedVersion)
        {
            var events = aggregate.GetUncommittedChanges();
            var streamName = typeof(T).FullName + "--" + aggregate.Id;
            using (var connection = EventStoreConnection.Create(_endPoint))
            {
                connection.Connect();
                foreach (var @event in events)
                {
                    var stringVersion = Newtonsoft.Json.JsonConvert.SerializeObject(@event);
                    var eventType = @event.GetType().Name;
                    var metadata = new byte[0];

                    var eventId = Guid.NewGuid();
                    var data = Encoding.UTF8.GetBytes(stringVersion);
                    var esEv = new EventData(eventId, eventType, true, data, metadata);

                    connection.AppendToStream(streamName, expectedVersion, new[] { esEv }, _credentials);

                    expectedVersion = ChangeExpectedVersion(expectedVersion);
                    PublishEvent(@event);
                }
            }
            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(string id)
        {
            return GetById(id, false);
        }

        public T GetById(string id, bool keepHistory)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var streamName = typeof(T).FullName + "--" + id;
            const int batchSize = 50;
            var startPosition = 0;
            var obj = new T();
            using (var connection = EventStoreConnection.Create(_endPoint))
            {
                connection.Connect();
                StreamEventsSlice evStream;
                try
                {
                    evStream = connection.ReadStreamEventsForward(streamName, startPosition, batchSize, false, _credentials);
                }
                catch (Exception)
                {
                    //Does not exist
                    return null;
                }
                while (evStream.Events != null && evStream.Events.Length > 0)
                {
                    var events = new List<Event>();
                    foreach (var ev in evStream.Events)
                    {
                        var stringVersion = Encoding.UTF8.GetString(ev.Event.Data);
                        var t = Type.GetType(ev.Event.EventType);
                        if (t != null)
                        {
                            var deserializedEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(stringVersion, t);
                            if (deserializedEvent is Event)
                            {
                                events.Add((Event)deserializedEvent);
                            }
                        }
                    }
                    obj.LoadsFromHistory(events, keepHistory);
                    startPosition += batchSize;
                    evStream = connection.ReadStreamEventsForward(streamName, startPosition, batchSize, false, _credentials);
                }
            }
            if (obj.Id != id)
            {
                return null;
            }
            return obj;
        }

        private static System.Net.IPEndPoint GetIpEndPoint(string serverName, int portNumber = 1113)
        {
            var addresses = System.Net.Dns.GetHostAddresses(serverName);
            if (addresses.Length == 0) throw new Exception(serverName);
            return new System.Net.IPEndPoint(addresses[0], portNumber);
        }

        private void PublishEvent(Event @event)
        {
            if (_publishers == null) return;
            foreach (var eventPublisher in _publishers)
            {
                if (eventPublisher != null) eventPublisher.Publish(@event);
            }
        }

        private static int ChangeExpectedVersion(int expectedVersion)
        {
            if (expectedVersion != -2)
            {
                expectedVersion++;
            }
            return expectedVersion;
        }
    }
}
