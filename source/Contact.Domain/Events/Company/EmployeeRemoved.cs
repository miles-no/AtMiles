using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
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
