using System;
using System.Runtime.Serialization;

namespace no.miles.at.Backend.Domain.Exceptions
{
    public abstract class DomainBaseException : Exception
    {
        public abstract string GetExceptionName();
        protected DomainBaseException(string message) : base(message) { }
        protected DomainBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
