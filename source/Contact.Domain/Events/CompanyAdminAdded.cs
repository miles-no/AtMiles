using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class CompanyAdminAdded : Event
    {
        public CompanyAdminAdded(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
