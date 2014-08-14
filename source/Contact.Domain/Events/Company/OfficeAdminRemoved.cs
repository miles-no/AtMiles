using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class OfficeAdminRemoved : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly string AdminId;
        public readonly string AdminName;

        public OfficeAdminRemoved(string companyId, string companyName, string officeId, string officeName, string adminId, string adminName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            AdminId = adminId;
            AdminName = adminName;
        }
    }
}
