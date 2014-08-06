using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class EmployeeTerminated : Event
    {
        public EmployeeTerminated(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
