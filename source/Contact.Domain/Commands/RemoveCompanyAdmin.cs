using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class RemoveCompanyAdmin : Command
    {
        public readonly string CompanyId;
        public readonly string AdminId;

        public RemoveCompanyAdmin(string companyId, string adminId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            :base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            AdminId = adminId;
        }
    }
}
