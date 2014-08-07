using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class UnknownItemException : DomainBaseException
    {
        public UnknownItemException() { }
        public UnknownItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
