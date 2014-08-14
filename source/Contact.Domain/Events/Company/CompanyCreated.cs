using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
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
