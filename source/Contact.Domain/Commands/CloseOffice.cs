using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class CloseOffice : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;

        public CloseOffice(string companyId, string officeId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
        }
    }
}
