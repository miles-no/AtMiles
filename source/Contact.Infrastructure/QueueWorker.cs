using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Contact.Infrastructure
{
    public class QueueWorker : LongRunningProcess
    {
        private readonly string _rabbitMqHostname;
        private readonly string _rabbitMqUsername;
        private readonly string _rabbitMqPassword;
        private readonly string _rabbitMqQueueName;
        private const int RabbitMqRequestedHeartbeat = 30;
        private Subscription _brokerSubscription;
        private IConnection _brokerConnection;
        private IModel _brokerChannel;
        private readonly Action<string, byte[], string, IDictionary<string, object>> _messageHandler;
        private const string MessageTypeDefinition = "type";
        private ConnectionFactory _factory;

        public QueueWorker(string rabbitMqHostname, string rabbitMqUsername, string rabbitMqPassword, string rabbitMqQueueName, ILog log, Action<string, byte[], string, IDictionary<string, object>> messageHandler)
            : base(log)
        {
            _rabbitMqHostname = rabbitMqHostname;
            _rabbitMqUsername = rabbitMqUsername;
            _rabbitMqPassword = rabbitMqPassword;
            _rabbitMqQueueName = rabbitMqQueueName;
            _messageHandler = messageHandler;
        }

        protected override void Initialize()
        {
            _factory = new ConnectionFactory
            {
                HostName = _rabbitMqHostname,
                UserName = _rabbitMqUsername,
                Password = _rabbitMqPassword,
                RequestedHeartbeat = RabbitMqRequestedHeartbeat
            };
        }

        protected override void Run()
        {
            while (IsRunning)
            {
                try
                {
                    OpenSubscription(_factory);
                    BasicDeliverEventArgs args;
                    bool gotMessage = _brokerSubscription.Next(250, out args);
                    if (gotMessage)
                    {
                        if (args == null)
                        {
                            Log.Warn("Connection to Broker closed.");
                            DisposeAllConnectionObjects();
                            continue;
                        }
                        try
                        {
                            var headers = GetMessageHeader(args);
                            string type = GetMessageType(headers);
                            _messageHandler(args.RoutingKey, args.Body, type, headers);
                        }
                        catch (Exception error)
                        {
                            Log.Error("Error handling message: ", error);
                        }
                        finally
                        {
                            _brokerSubscription.Ack(args);
                        }
                    }
                }
                catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
                {
                    Log.Error("Broker operation was interrupted.", ex);
                    DisposeAllConnectionObjects();
                }
                catch (Exception error)
                {
                    Log.Error("Broker experienced unknown error.", error);
                    DisposeAllConnectionObjects();
                }
            }
            DisposeAllConnectionObjects();
        }

        private static IDictionary<string, object> GetMessageHeader(BasicDeliverEventArgs args)
        {
            if (args == null) return new Dictionary<string, object>();
            if (args.BasicProperties == null) return new Dictionary<string, object>(); ;
            return args.BasicProperties.Headers;
        }

        private static string GetMessageType(IDictionary<string, object> headers)
        {
            if (headers == null) return string.Empty;
            if (!headers.ContainsKey(MessageTypeDefinition)) return string.Empty;

            var typeAsBytes = (byte[])headers[MessageTypeDefinition];
            return Encoding.UTF8.GetString(typeAsBytes);
        }

        private void OpenSubscription(ConnectionFactory factory)
        {
            if (_brokerSubscription == null)
            {
                try
                {
                    _brokerConnection = factory.CreateConnection();
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException error)
                {
                    Log.Error("Not able to conenct to Broker.", error);
                    Thread.Sleep(5000);
                }
                _brokerChannel = _brokerConnection.CreateModel();
                CreateQueueIfNotExisting(_brokerChannel, _rabbitMqQueueName);

                //RV: 28.11.2013: Must be activated if we have more than one running host competing to serve the queue
                //_brokerChannel.BasicQos(0, 1, false);

                _brokerSubscription = new Subscription(_brokerChannel, _rabbitMqQueueName, false);
            }
        }

        private static void CreateQueueIfNotExisting(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, true, false, false, null);
        }

        private void DisposeAllConnectionObjects()
        {
            if (_brokerSubscription != null)
            {
                ((IDisposable)_brokerSubscription).Dispose();
                _brokerSubscription = null;
            }

            if (_brokerChannel != null)
            {
                _brokerChannel.Dispose();
                _brokerChannel = null;
            }

            if (_brokerConnection != null)
            {
                try
                {
                    _brokerConnection.Dispose();
                }
                catch { }
                _brokerConnection = null;
            }
        }
    }
}
