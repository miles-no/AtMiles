using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class OfficeOpened : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly Address Address;

        public OfficeOpened(string companyId, string companyName, string officeId, string officeName, Address address, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            Address = address;
        }
    }
}
