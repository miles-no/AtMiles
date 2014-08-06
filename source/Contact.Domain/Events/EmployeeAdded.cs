using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class EmployeeAdded : Event
    {
        public EmployeeAdded(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
