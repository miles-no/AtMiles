using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class TerminateEmployee : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;

        public TerminateEmployee(string companyId, string employeeId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
        }
    }
}
