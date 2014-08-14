using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class OfficeClosed : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;

        public OfficeClosed(string companyId, string companyName, string officeId, string officeName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
        }
    }
}
