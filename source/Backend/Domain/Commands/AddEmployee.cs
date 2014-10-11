using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class AddEmployee : Command
    {
        public readonly string CompanyId;
        public readonly string GlobalId;
        public readonly Login LoginId;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;

        public AddEmployee(string companyId, string globalId, Login loginId, string firstName, string middleName, string lastName, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            :base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            GlobalId = globalId;
            LoginId = loginId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }
    }
}
