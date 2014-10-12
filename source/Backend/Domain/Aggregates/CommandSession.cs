using System;
using no.miles.at.Backend.Domain.Annotations;
using no.miles.at.Backend.Domain.Events.CommandSession;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Aggregates
{
    public class CommandSession : AggregateRoot
    {
        private const string SessionStreamId = "CommandSessions";

        public CommandSession()
        {
            _id = SessionStreamId;
        }

        public void AddRequestCommand(Command command)
        {
            var ev = new CommandRequested(command.GetType().Name, DateTime.UtcNow, command.CreatedBy, command.CorrelationId);
            ApplyChange(ev);
        }

        public void MarkCommandAsSuccess(Person createdBy, string correlationId)
        {
            var ev = new CommandSucceded(DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        public void AddException(DomainBaseException domainException, Person createdBy, string correlationId)
        {
            var ev = new CommandException(domainException, DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CommandRequested ev)
        {
            //TODO: Do nothing for now
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CommandSucceded ev)
        {
            //TODO: Do nothing for now
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CommandException ev)
        {
            //TODO: Do nothing for now
        }
    }
}
