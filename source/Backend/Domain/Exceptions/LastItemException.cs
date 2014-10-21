using System.Runtime.Serialization;

namespace no.miles.at.Backend.Domain.Exceptions
{
    public class LastItemException : DomainBaseException
    {
        public LastItemException(string message) : base(message) { }
        public LastItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "Last item";
        }
    }
}
