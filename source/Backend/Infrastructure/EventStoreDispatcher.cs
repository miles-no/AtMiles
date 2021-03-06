﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using no.miles.at.Backend.Domain;

namespace no.miles.at.Backend.Infrastructure
{
    public class EventStoreDispatcher : LongRunningProcess
    {
        private EventStoreCatchUpSubscription _subscription;
        private IEventStoreConnection _eventStoreConnection;
        private readonly string _eventStoreServer;
        private readonly string _eventStoreUsername;
        private readonly string _eventStorePassword;
        private readonly IEventPublisher _publisher;
        private readonly Action _liveProcessing;
        private readonly IHandlePosition _positionHandler;

        public EventStoreDispatcher(string eventStoreServer, string eventStoreUsername, string eventStorePassword, IEventPublisher publisher, ILog log, Action liveProcessing, IHandlePosition positionHandler)
            : base(log)
        {
            _eventStoreServer = eventStoreServer;
            _eventStoreUsername = eventStoreUsername;
            _eventStorePassword = eventStorePassword;
            _publisher = publisher;
            _liveProcessing = liveProcessing;
            _positionHandler = positionHandler;
        }

        protected override async Task Initialize()
        {
            var settings = ConnectionSettings.Create();
            var endPoint = GetIpEndPoint(_eventStoreServer);
            _eventStoreConnection = EventStoreConnection.Create(settings, endPoint);
            await _eventStoreConnection.ConnectAsync();
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
            PublishEvent(resolvedEvent);
            SavePosition(resolvedEvent.OriginalPosition);
        }

        private void PublishEvent(ResolvedEvent resolvedEvent)
        {
            Event domainEvent;
            if (TryDeserializeAggregateEvent(resolvedEvent, out domainEvent))
            {
                try
                {
                    _publisher.Publish(domainEvent);
                }
                catch (Exception error)
                {
                    Log.Warn("Not able to publish event", error);
                }
            }
        }

        private void SavePosition(Position? originalPosition)
        {
            try
            {
                if (_positionHandler != null && originalPosition.HasValue)
                {
                    _positionHandler.SaveLatestPosition(new EventStoreGlobalPosition
                    {
                        PreparePosition = originalPosition.Value.PreparePosition,
                        CommitPosition = originalPosition.Value.CommitPosition
                    });
                }
            }
            catch (Exception error)
            {
                Log.Warn("Not able to save lastest position", error);
            }
        }

        private void DroppedHandle(EventStoreCatchUpSubscription arg1, SubscriptionDropReason dropReason, Exception exception)
        {
            if (dropReason == SubscriptionDropReason.UserInitiated) return;

            if (exception != null)
            {
                Log.Warn("Connection to EventStore dropped. Reason: " + dropReason, exception);
            }
            else
            {
                Log.Warn("Connection to EventStore dropped. Reason: " + dropReason);
            }
            Thread.Sleep(10000);
            RecoverSubscription();
        }

        private Position? GetLatestPosition()
        {
            if (_positionHandler == null) return null;

            var position = _positionHandler.GetLatestSavedPosition();
            if (position == null) return null;

            return new Position(position.CommitPosition, position.PreparePosition);
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription arg1)
        {
            Log.Info("TimeLine starting live processing.");
            if (_liveProcessing != null)
            {
                _liveProcessing();
            }
        }

        private static System.Net.IPEndPoint GetIpEndPoint(string serverName, int portNumber = 1113)
        {
            var addresses = System.Net.Dns.GetHostAddresses(serverName);
            if (addresses.Length == 0) throw new Exception(serverName);
            return new System.Net.IPEndPoint(addresses[0], portNumber);
        }

        private bool TryDeserializeAggregateEvent(ResolvedEvent rawEvent, out Event deserializedEvent)
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
            catch (Exception error)
            {
                Log.Warn("Exception deserializing event", error);
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
