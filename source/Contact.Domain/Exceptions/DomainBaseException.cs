using System;
using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class DomainBaseException : Exception
    {
        public DomainBaseException(string message) : base(message) { }
        public DomainBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
