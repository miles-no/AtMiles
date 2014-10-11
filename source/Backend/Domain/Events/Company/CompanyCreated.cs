using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Company
{
    public class CompanyCreated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;

        public CompanyCreated(string companyId, string companyName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
        }
    }
}
