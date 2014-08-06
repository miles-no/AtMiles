using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class OfficeClosed : Event
    {
        public OfficeClosed(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
