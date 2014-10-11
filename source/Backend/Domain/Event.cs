using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain
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
