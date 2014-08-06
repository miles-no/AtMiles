using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public abstract class Command : Message
    {
        private readonly DateTime _created;
        private readonly Person _createdBy;
        private readonly String _correlationId;
        private readonly Int32 _basedOnVersion;

        public DateTime Created { get { return _created; } }
        public Person CreatedBy { get { return _createdBy; } }
        public String CorrelationId { get { return _correlationId; } }
        public Int32 BasedOnVersion { get { return _basedOnVersion; } }

        protected Command(DateTime created, Person createdBy, String correlationId, Int32 basedOnVersion)
        {
            _created = created;
            _createdBy = createdBy;
            _correlationId = correlationId;
            _basedOnVersion = basedOnVersion;
        }
    }
}
