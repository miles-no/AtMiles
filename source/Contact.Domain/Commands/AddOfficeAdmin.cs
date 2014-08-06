using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddOfficeAdmin : Command
    {
        public string AdminId { get; private set; }
        public string OfficeId { get; private set; }

        public AddOfficeAdmin(string adminId, string officeId)
        {
            AdminId = adminId;
            OfficeId = officeId;
        }
    }
}
