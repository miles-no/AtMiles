using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class OpenOffice : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly Address Address;

        public OpenOffice(string companyId, string officeId, string officeName, Address address, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            OfficeName = officeName;
            Address = address;
        }
    }
}
