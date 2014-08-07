using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public abstract class Command : Message
    {
        private DateTime _created;
        private Person _createdBy;
        private String _correlationId;
        private Int32 _basedOnVersion;

        public DateTime Created { get { return _created; } }
        public Person CreatedBy { get { return _createdBy; } }
        public String CorrelationId { get { return _correlationId; } }
        public Int32 BasedOnVersion { get { return _basedOnVersion; } }

        protected Command()
        {
            _created = DateTime.UtcNow;
            _createdBy = new Person("?", "Unknown");
            _correlationId = Guid.NewGuid().ToString();
            _basedOnVersion = Constants.IgnoreVersion;
        }

        public Command WithCreated(DateTime created)
        {
            _created = created;
            return this;
        }

        public Command WithCreatedBy(Person createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public Command WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        public Command WithBasedOnVersion(int basedOnVersion)
        {
            _basedOnVersion = basedOnVersion;
            return this;
        }
    }
}
