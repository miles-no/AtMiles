using System;
using Contact.Domain.Annotations;
using Contact.Domain.Events.CommandSession;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Aggregates
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
            var ev = new CommandRequested(command);
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
