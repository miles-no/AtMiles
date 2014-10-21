using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Company
{
    public class EmployeeAdded : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string GlobalId;
        public readonly string Name;
        public readonly Login Login;

        public EmployeeAdded(string companyId, string companyName, string globalId, string name, Login login, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            GlobalId = globalId;
            Name = name;
            Login = login;
        }
    }
}
