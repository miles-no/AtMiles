using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class OpenOffice : Command
    {
        public OpenOffice(DateTime created, Person createdBy, String correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            //TODO: Implement
        }
    }
}
