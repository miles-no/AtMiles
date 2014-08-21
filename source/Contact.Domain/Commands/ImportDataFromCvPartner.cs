using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class ImportDataFromCvPartner : Command
    {
        public readonly string CompanyId;

        public ImportDataFromCvPartner(string companyId, DateTime created, Person createdBy, string correlationId, int basedOnVersion) :
            base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
        }
    }
}
