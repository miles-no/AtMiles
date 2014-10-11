using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class CompanyAdminRemoved : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string AdminId;
        public readonly string AdminName;

        public CompanyAdminRemoved(string companyId, string companyName, string adminId, string adminName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            AdminId = adminId;
            AdminName = adminName;
        }
    }
}
