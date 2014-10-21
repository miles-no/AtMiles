using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Company
{

    public class EmployeeRemoved : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string Id;
        public readonly string Name;

        public EmployeeRemoved(string companyId, string companyName, string id, string name, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            Id = id;
            Name = name;
        }
    }
}
