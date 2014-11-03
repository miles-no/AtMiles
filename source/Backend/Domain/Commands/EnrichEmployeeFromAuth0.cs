using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class EnrichEmployeeFromAuth0 : Command
    {
        public readonly string CompanyId;
        public readonly string Email;
        public EnrichEmployeeFromAuth0(string companyId, string email, DateTime created, Person createdBy, string correlationId, int basedOnVersion): base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            Email = email;
        }
        
    }
}