using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class LastItemException : DomainBaseException
    {
        public LastItemException(){}
        public LastItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
