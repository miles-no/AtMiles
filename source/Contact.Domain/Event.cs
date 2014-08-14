using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public abstract class Event : Message
    {
        public readonly DateTime Created;
        public readonly Person CreatedBy;
        public readonly String CorrelationId;

        protected Event(DateTime created, Person createdBy, string correlationId)
        {
            Created = created;
            CreatedBy = createdBy;
            CorrelationId = correlationId;
        }
    }
}
