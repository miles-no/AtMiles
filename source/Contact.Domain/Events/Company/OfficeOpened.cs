using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class OfficeOpened : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }

        public string OfficeName { get; private set; }

        public Address Address { get; private set; }

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
