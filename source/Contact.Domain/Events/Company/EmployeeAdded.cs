using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class EmployeeAdded : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly string GlobalId;
        public readonly string Name;

        public EmployeeAdded(string companyId, string companyName, string officeId, string officeName, string globalId, string name, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            GlobalId = globalId;
            Name = name;
        }
    }
}
