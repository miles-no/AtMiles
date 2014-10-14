using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain;
using RabbitMQ.Client;

namespace no.miles.at.Backend.Infrastructure
{
    public sealed class RabbitMqCommandSender : ICommandSender, IDisposable
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _exchangeName;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private readonly ushort _heartbeatInterval;
        private readonly bool _useSsl;

        public RabbitMqCommandSender(string hostName, string username, string password, string exchangeName, bool useSsl = false)
        {
            _hostName = hostName;
            _username = username;
            _password = password;
            _exchangeName = exchangeName;
            _useSsl = useSsl;
            _heartbeatInterval = 30;

            Connect();
        }

        ~RabbitMqCommandSender()
        {
            Dispose();
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(obj: this);
        }

        public void Send<T>(T command) where T : Command
        {
            if (!IsConnected())
            {
                Connect();
            }
            if (IsConnected())
            {
                {
                    var properties = _model.CreateBasicProperties();
                    properties.SetPersistent(true);
                    properties.Headers = new Dictionary<string, object>
                    {
                        {"type", command.GetType().Name},
                        {"clrtype", command.GetType().AssemblyQualifiedName},
                        {"correlationid", command.CorrelationId}
                    };
                    string routingKey = GetRoutingKey();
                    try
                    {
                        byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
                        _model.BasicPublish(_exchangeName, routingKey, properties, body);
                    }
                    catch (Exception error)
                    {
                        throw new Exception("Not able to send to queue", error);
                    }
                }
            }
            //else
            //{
                //TODO: Log error
            //}
        }

        private string GetRoutingKey()
        {
            return "";
        }

        private bool Connect()
        {
            if (_useSsl)
            {
                var ssl = new SslOption {Enabled = true, ServerName = _hostName};

                _connectionFactory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password,
                    RequestedHeartbeat = _heartbeatInterval,
                    Ssl = ssl
                };
            }
            else
            {
                _connectionFactory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password,
                    RequestedHeartbeat = _heartbeatInterval
                };
            }
            try
            {
                _connection = _connectionFactory.CreateConnection();

                if (_connection != null && _connection.IsOpen)
                {
                    _model = _connection.CreateModel();
                    return _model != null && _model.IsOpen;
                }
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            { }
            return false;  // Failed to create connection
        }

        private void Disconnect()
        {
            if (_model != null)
            {
                lock (_model)
                {
                    _model.Close(200, "Goodbye");
                    _model.Dispose();
                }
                _model = null;
            }

            if (_connection != null)
            {
                lock (_connection)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                _connection = null;
            }

            if (_connectionFactory != null)
            {
                lock (_connectionFactory)
                {
                    _connectionFactory = null;
                }
            }
        }

        private bool IsConnected()
        {
            if (_connectionFactory == null) return false;
            if (_connection == null) return false;
            if (_model == null) return false;
            return _model.IsOpen;
        }
    }
}
