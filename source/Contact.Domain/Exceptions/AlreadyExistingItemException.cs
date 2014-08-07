using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class AlreadyExistingItemException : DomainBaseException
    {
        public AlreadyExistingItemException() { }
        public AlreadyExistingItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
