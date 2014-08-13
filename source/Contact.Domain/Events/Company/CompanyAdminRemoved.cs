using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class CompanyAdminRemoved : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string AdminId { get; private set; }
        public string AdminName { get; private set; }

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
