using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Exceptions;

namespace no.miles.at.Backend.Infrastructure
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


        public async Task MessageHandler(string routingKey, byte[] messageBody, string messageType,
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
                    DomainBaseException capturedException = null;
                    Exception internalException = null;
                    try
                    {
                        session.AddRequestCommand(command);
                        //Call save to make sure to get info on long running processes
                        await _commandSessionRepository.SaveAsync(session, Constants.IgnoreVersion);

                        await _handler.HandleCommand(command, t);
                        session.MarkCommandAsSuccess(command.CreatedBy, command.CorrelationId);
                    }
                    catch (DomainBaseException domainException)
                    {
                        capturedException = domainException;
                    }
                    catch (Exception error)
                    {
                        internalException = error;
                    }

                    if (capturedException != null)
                    {
                        session.AddException(capturedException, command.CreatedBy, command.CorrelationId);
                    }
                    if (internalException != null)
                    {
                        session.AddException(new ValueException("Internal server error"), command.CreatedBy, command.CorrelationId);
                        //TODO: Log error
                    }

                    await _commandSessionRepository.SaveAsync(session, Constants.IgnoreVersion);
                }
            }
        }
    }
}
