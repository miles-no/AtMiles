using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class TerminateEmployee : Command
    {
        public TerminateEmployee(DateTime created, Person createdBy, String correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            //TODO: Implement
        }
    }
}
