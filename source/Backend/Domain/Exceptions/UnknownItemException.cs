using System.Runtime.Serialization;

namespace no.miles.at.Backend.Domain.Exceptions
{
    public class UnknownItemException : DomainBaseException
    {
        public UnknownItemException(string message) : base(message) { }
        public UnknownItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "Unknown Item";
        }
    }

    public class UnknownUserException : DomainBaseException
    {
        public UnknownUserException(string message) : base(message)
        {

        }

        public UnknownUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string GetExceptionName()
        {
            return "Unknown user";
        }
    }
}
