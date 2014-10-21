using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain
{
    public abstract class Command : Message
    {
        public readonly DateTime Created;
        public readonly Person CreatedBy;
        public readonly String CorrelationId;
        public readonly Int32 BasedOnVersion;

        protected Command(DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
        {
            Created = created;
            CreatedBy = createdBy;
            CorrelationId = correlationId;
            BasedOnVersion = basedOnVersion;
        }
    }
}
