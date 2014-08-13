using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class OpenOffice : Command
    {
        public string CompanyId { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }

        public OpenOffice(string companyId, string name, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            Name = name;
            Address = null;
        }

        public OpenOffice WithAddress(Address address)
        {
            Address = address;
            return this;
        }
    }
}
