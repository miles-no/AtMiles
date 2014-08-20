using System;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;
using Newtonsoft.Json;

namespace Contact.Domain.Events.CommandSession
{
    public class CommandException : Event
    {
        public readonly string ExceptionName;
        public readonly string ExceptionMessage;

        [JsonConstructor]
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

