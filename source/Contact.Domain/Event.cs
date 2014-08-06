using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public abstract class Event : Message
    {
        private readonly DateTime _created;
        private readonly Person _createdBy;
        private readonly String _correlationId;

        public DateTime Created { get { return _created; } }
        public Person CreatedBy { get { return _createdBy; } }
        public String CorrelationId { get { return _correlationId; } }

        protected Event(DateTime created, Person createdBy, String correlationId)
        {
            _created = created;
            _createdBy = createdBy;
            _correlationId = correlationId;
        }

    }
}
