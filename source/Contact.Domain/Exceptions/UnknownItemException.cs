using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
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
}
