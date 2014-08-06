using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class CompanyAdminRemoved : Event
    {
        public CompanyAdminRemoved(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
