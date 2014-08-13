using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class RemoveCompanyAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string AdminId { get; private set; }

        public RemoveCompanyAdmin(string companyId, string adminId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            :base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            AdminId = adminId;
        }
    }
}
