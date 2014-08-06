using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class OfficeOpened : Event
    {
        public OfficeOpened(DateTime created, Person createdBy, String correlationId)
            : base(created, createdBy, correlationId)
        {
            //TODO: Implement
        }
    }
}
