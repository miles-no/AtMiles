using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class OfficeAdminAdded : Event
    {
        public OfficeAdminAdded(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
