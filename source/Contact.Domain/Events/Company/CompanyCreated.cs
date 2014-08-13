using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class CompanyCreated : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }

        public CompanyCreated(string companyId, string companyName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
        }
    }
}
