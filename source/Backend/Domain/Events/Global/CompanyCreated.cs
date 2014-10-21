using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Global
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
