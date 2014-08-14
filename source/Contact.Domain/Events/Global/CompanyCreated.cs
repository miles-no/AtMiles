using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Global
{
    public class CompanyCreated : Event
    {
        public readonly string Id;
        public readonly string Name;

        public CompanyCreated(string id, string name, DateTime created, Person createdBy, string correlationId)
            :base(created, createdBy, correlationId)
        {
            Id = id;
            Name = name;
        }
    }
}
