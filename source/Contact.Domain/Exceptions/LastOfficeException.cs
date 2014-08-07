using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class LastOfficeException : DomainBaseException
    {
        public LastOfficeException(){}
        public LastOfficeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
