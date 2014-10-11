using System.Runtime.Serialization;

namespace no.miles.at.Backend.Domain.Exceptions
{
    public class AlreadyExistingItemException : DomainBaseException
    {
        public AlreadyExistingItemException(string message) : base(message) { }
        public AlreadyExistingItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "Already existing item";
        }
    }
}
