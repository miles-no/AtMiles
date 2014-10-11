using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.CommandSession
{
    public class CommandSucceded : Event
    {
        public CommandSucceded(DateTime created, Person createdBy, string correlationId)
            :base(created, createdBy, correlationId){}
    }
}
