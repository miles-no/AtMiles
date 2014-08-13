using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;

namespace Contact.Infrastructure
{
    public class RabbitMqCommandHandler
    {
        private MainCommandHandler _handler;
        public RabbitMqCommandHandler(MainCommandHandler handler)
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

        public void StartReceiving()
        {
            //TODO: Implement
        }

        public void StopReceiving()
        {
            //TODO: Implement
        }
    }
}
