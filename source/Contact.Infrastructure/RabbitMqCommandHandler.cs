using System;
using System.Collections.Generic;
using System.Text;
using Contact.Domain;
using Contact.Domain.CommandHandlers;

namespace Contact.Infrastructure
{
    public class RabbitMqCommandHandler
    {
        private readonly MainCommandHandler _handler;
        public RabbitMqCommandHandler(MainCommandHandler handler)
        {
            _handler = handler;
        }


        public void MessageHandler(string routingKey, byte[] messageBody, string messageType,
            IDictionary<string, object> headers)
        {
            if (messageType == null) throw new Exception("Type not set for event");

            var stringVersion = Encoding.UTF8.GetString(messageBody);
            var t = Type.GetType(messageType);
            if (t != null)
            {
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject(stringVersion, t);
                if (command is Command)
                {
                    _handler.HandleCommand((Command)command, t);
                }
            }
        }
    }
}
