using System;
using System.Collections.Generic;
using RabbitMQ.Client.Impl;

namespace Contact.Infrastructure
{
    public class RabbitMqCommandHandler
    {
        private CommandHandler _handler;
        public RabbitMqCommandHandler(CommandHandler handler)
        {
            _handler = handler;
        }


        public void MessageHandler(string routingKey, byte[] messageBody, string messageType,
            IDictionary<string, object> headers)
        {
            if (messageType == null) throw new Exception("Type not set for event");

            //TODO: Ckeck type

            //TODO: deserialize

            //TODO: Handle
        }
    }
}
