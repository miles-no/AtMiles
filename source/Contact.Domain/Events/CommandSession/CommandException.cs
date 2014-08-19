using System;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.CommandSession
{
    public class CommandException : Event
    {
        public readonly string ExceptionName;
        public readonly string ExceptionMessage;

        public CommandException(string exceptionName, string exceptionMessage, DateTime created, Person createdBy,
            string correlationId)
            : base(created, createdBy, correlationId)
        {
            ExceptionName = exceptionName;
            ExceptionMessage = exceptionMessage;
        }

        public CommandException(DomainBaseException exception, DateTime created, Person createdBy,
            string correlationId)
            : base(created, createdBy, correlationId)
        {
            if (exception != null)
            {
                ExceptionName = exception.GetExceptionName();
                ExceptionMessage = exception.Message;
            }
        }
    }
}

