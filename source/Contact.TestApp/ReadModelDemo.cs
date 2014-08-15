using System;
using System.Threading;
using Contact.Domain;
using Contact.Infrastructure;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Contact.TestApp
{
    public class ReadModelDemo : LongRunningProcess
    {
        private EventStoreCatchUpSubscription _subscription;
        private IEventStoreConnection _eventStoreConnection;
        private IEventPublisher _timeLineHandler;
        private readonly string _eventStoreServer;
        private readonly string _eventStoreUsername;
        private readonly string _eventStorePassword;

        public ReadModelDemo(string eventStoreServer, string eventStoreUsername, string eventStorePassword, ILog log)
            : base(log)
        {
            _eventStoreServer = eventStoreServer;
            _eventStoreUsername = eventStoreUsername;
            _eventStorePassword = eventStorePassword;
        }

        public ReadModelDemo(ILog log) : base(log)
        {

        }

        protected override void Initialize()
        {
            var endPoint = GetIpEndPoint(_eventStoreServer);
            _eventStoreConnection = EventStoreConnection.Create(endPoint);
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
                        LiveProcessingStarted, DroppedHandle, credentials);
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

        private void EventHandle(EventStoreCatchUpSubscription arg1, ResolvedEvent ev)
        {
            try
            {
                var stringVersion = System.Text.Encoding.UTF8.GetString(ev.Event.Data);

                string eventType = ev.Event.EventType;

                
                //handle new types here

            }
            catch (Exception exception)
            {
                Log.Error("Error processing event", exception);
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
    }
}
