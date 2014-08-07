using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddOfficeAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string OfficeId { get; private set; }
        public string AdminId { get; private set; }


        public AddOfficeAdmin(string companyId, string officeId, string adminId)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            AdminId = adminId;
        }
    }
}
