using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class NoAccessException : DomainBaseException
    {
        public NoAccessException(string message) : base(message) { }
        public NoAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
