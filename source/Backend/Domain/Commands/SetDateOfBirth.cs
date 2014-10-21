using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class SetDateOfBirth : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly DateTime DateOfBirth;

        public SetDateOfBirth(string companyId, string employeeId, DateTime dateOfBirth, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            DateOfBirth = dateOfBirth;
        }
    }
}
