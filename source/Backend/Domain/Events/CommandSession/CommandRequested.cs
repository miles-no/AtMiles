using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.CommandSession
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
