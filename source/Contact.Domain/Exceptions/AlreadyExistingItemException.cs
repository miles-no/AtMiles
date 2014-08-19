using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class AlreadyExistingItemException : DomainBaseException
    {
        public AlreadyExistingItemException(string message) : base(message) { }
        public AlreadyExistingItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
