using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class OpenOffice : Command
    {
        public readonly string CompanyId;
        public readonly string Name;
        public readonly Address Address;

        public OpenOffice(string companyId, string name, Address address, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            Name = name;
            Address = address;
        }
    }
}
