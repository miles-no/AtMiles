using System.Runtime.Serialization;

namespace no.miles.at.Backend.Domain.Exceptions
{
    public class NoAccessException : DomainBaseException
    {
        public NoAccessException(string message) : base(message) { }
        public NoAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "No Access";
        }
    }
}
