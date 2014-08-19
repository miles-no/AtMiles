using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class ExistingChildItemsException : DomainBaseException
    {
        public ExistingChildItemsException(string message) : base(message) { }
        public ExistingChildItemsException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "Existing child item(s)";
        }
    }
}
