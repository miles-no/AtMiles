using System;
using System.Collections.Generic;
using System.Text;
using Contact.Domain;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Exceptions;

namespace Contact.Infrastructure
{
    public class RabbitMqCommandHandler
    {
        private readonly MainCommandHandler _handler;
        private readonly IRepository<CommandSession> _commandSessionRepository;
        public RabbitMqCommandHandler(MainCommandHandler handler, IRepository<CommandSession> commandSessionRepository)
        {
            _handler = handler;
            _commandSessionRepository = commandSessionRepository;
        }


        public void MessageHandler(string routingKey, byte[] messageBody, string messageType,
            IDictionary<string, object> headers)
        {
            if (messageType == null) throw new Exception("Type not set for event");

            var stringVersion = Encoding.UTF8.GetString(messageBody);
            var t = Type.GetType(messageType);
            if (t != null)
            {
                var rawCommand = Newtonsoft.Json.JsonConvert.DeserializeObject(stringVersion, t);
                var command = rawCommand as Command;
                if (command != null)
                {
                    var session = new CommandSession();
                    try
                    {
                        session.AddRequestCommand(command);
                        _handler.HandleCommand(command, t);
                        session.MarkCommandAsSuccess(command.CreatedBy, command.CorrelationId);
                    }
                    catch (DomainBaseException domainException)
                    {
                        session.AddException(domainException, command.CreatedBy, command.CorrelationId);
                    }
                    finally
                    {
                        _commandSessionRepository.Save(session, Constants.IgnoreVersion);
                    }
                }
            }
        }
    }
}
