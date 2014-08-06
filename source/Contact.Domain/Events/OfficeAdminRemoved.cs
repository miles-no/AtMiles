using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class OfficeAdminRemoved : Event
    {
        public OfficeAdminRemoved(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
