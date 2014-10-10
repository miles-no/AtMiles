using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
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
