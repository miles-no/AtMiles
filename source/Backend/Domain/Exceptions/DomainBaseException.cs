using System;
using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public abstract class DomainBaseException : Exception
    {
        public abstract string GetExceptionName();
        public DomainBaseException(string message) : base(message) { }
        public DomainBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
