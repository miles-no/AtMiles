using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddCompanyAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string NewAdminId { get; private set; }

        public AddCompanyAdmin(string companyId, string newAdminId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            NewAdminId = newAdminId;
        }
    }
}
