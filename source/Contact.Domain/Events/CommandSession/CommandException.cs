using System;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.CommandSession
{
    public class CommandException : Event
    {
        public readonly DomainBaseException Exception;

        public CommandException(DomainBaseException exception, DateTime created, Person createdBy,
            string correlationId)
            : base(created, createdBy, correlationId)
        {
            Exception = exception;
        }
    }
}
