using System;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.CommandSession
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

