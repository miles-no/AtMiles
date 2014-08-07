using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class NoAccessException : DomainBaseException
    {
        public NoAccessException(){}
        public NoAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
