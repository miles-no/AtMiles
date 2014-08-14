using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddOfficeAdmin : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
        public readonly string AdminId;


        public AddOfficeAdmin(string companyId, string officeId, string adminId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            AdminId = adminId;
        }
    }
}
