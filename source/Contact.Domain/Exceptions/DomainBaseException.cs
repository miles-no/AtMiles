using System;
using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class DomainBaseException : Exception
    {
        public DomainBaseException() { }
        public DomainBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
