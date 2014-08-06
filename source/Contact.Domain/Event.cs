using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public abstract class Event : Message
    {
        private DateTime _created;
        private Person _createdBy;
        private String _correlationId;

        public DateTime Created { get { return _created; } }
        public Person CreatedBy { get { return _createdBy; } }
        public String CorrelationId { get { return _correlationId; } }

        protected Event()
        {
            _created = DateTime.UtcNow;
            _createdBy = new Person("?", "Unknown");
            _correlationId = Guid.NewGuid().ToString();
        }

        public Event WithCreated(DateTime created)
        {
            _created = created;
            return this;
        }

        public Event WithCreatedBy(Person createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public Event WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}
