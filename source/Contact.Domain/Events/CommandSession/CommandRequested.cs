using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.CommandSession
{
    public class CommandRequested : Event
    {
        public readonly string CommandName;

        public CommandRequested(string commandName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CommandName = commandName;
        }
    }
}
