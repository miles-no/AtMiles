using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class CompanyAdminAdded : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string NewAdminId;
        public readonly string NewAdminName;

        public CompanyAdminAdded(string companyId, string companyName, string newAdminId, string newAdminName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            NewAdminId = newAdminId;
            NewAdminName = newAdminName;
        }
    }
}
