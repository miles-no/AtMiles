using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.CommandSession
{
    public class CommandSucceded : Event
    {
        public CommandSucceded(DateTime created, Person createdBy, string correlationId)
            :base(created, createdBy, correlationId){}
    }
}
