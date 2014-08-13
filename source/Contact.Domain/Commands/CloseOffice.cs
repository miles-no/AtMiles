using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class CloseOffice : Command
    {
        public string CompanyId { get; private set; }
        public string OfficeId { get; private set; }

        public CloseOffice(string companyId, string officeId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
        }
    }
}
