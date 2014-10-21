using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
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
