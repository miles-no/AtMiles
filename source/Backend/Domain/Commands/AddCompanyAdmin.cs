using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class AddCompanyAdmin : Command
    {
        public readonly string CompanyId;
        public readonly string NewAdminId;

        public AddCompanyAdmin(string companyId, string newAdminId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            NewAdminId = newAdminId;
        }
    }
}
