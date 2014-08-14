using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class TerminateEmployee : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
        public readonly string EmployeeId;

        public TerminateEmployee(string companyId, string officeId, string employeeId, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            EmployeeId = employeeId;
        }
    }
}
