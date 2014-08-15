using Contact.Domain;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contact.Infrastructure
{
    public class EventStoreRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        //private const string CommitIdHeader = "CommitId";
        //private const int WritePageSize = 500;
        //private const int ReadPageSize = 500;

        private readonly Func<Type, string, string> _aggregateIdToStreamName;

        private readonly System.Net.IPEndPoint _endPoint;
        private readonly IEventPublisher _publisher;
        private readonly UserCredentials _credentials;


        

        public EventStoreRepository(string serverName, IEventPublisher publisher, string username, string password)
            : this(serverName, publisher, username, password, (t, g) => string.Format("{0}-{1}", char.ToLower(t.Name[0]) + t.Name.Substring(1), g))
        {
        }

        public EventStoreRepository(string serverName, IEventPublisher publisher, string username, string password, Func<Type, string, string> aggregateIdToStreamName)
        {
            _publisher = publisher;
            _endPoint = GetIpEndPoint(serverName);
            _credentials = new UserCredentials(username, password);
            _aggregateIdToStreamName = aggregateIdToStreamName;
        }

        public EventStoreRepository(string serverName, int portNumber, IEventPublisher publisher)
        {
            _endPoint = GetIpEndPoint(serverName, portNumber);
            _publisher = publisher;
        }

        public void Save(T aggregate, int expectedVersion)
        {
            var events = aggregate.GetUncommittedChanges();
            var streamName = _aggregateIdToStreamName(typeof(T), aggregate.Id);

            var headers = new Dictionary<string, object>
            {   
                {Constants.EventStoreAggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };

            using (var connection = EventStoreConnection.Create(_endPoint))
            {
                connection.Connect();
                foreach (var @event in events)
                {
                    var stringVersion = JsonConvert.SerializeObject(@event);
                    var eventType = @event.GetType().Name;
                    var metadata = AddEventClrTypeHeaderAndSerializeMetadata(@event, headers);

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

        private static byte[] AddEventClrTypeHeaderAndSerializeMetadata(object evnt, IDictionary<string, object> headers)
        {
            var eventHeaders = new Dictionary<string, object>(headers)
                {
                    {Constants.EventStoreEventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders));
        }

        public object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(Constants.EventStoreEventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
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
            var streamName = _aggregateIdToStreamName(typeof(T), id);
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
                        //DeserializeEvent(ev.OriginalEvent.Metadata, ev.OriginalEvent.Data);
                        //var stringVersion = Encoding.UTF8.GetString(ev.Event.Data);

                        //var t = Type.GetType(ev.Event.EventType);
                        //if (t != null)
                        //{
                            //var deserializedEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(stringVersion, t);
                            var deserializedEvent = DeserializeEvent(ev.OriginalEvent.Metadata, ev.OriginalEvent.Data);
                            if (deserializedEvent is Event)
                            {
                                events.Add((Event)deserializedEvent);
                            }
                        //}
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
            if (_publisher == null) return;
            _publisher.Publish(@event);
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
