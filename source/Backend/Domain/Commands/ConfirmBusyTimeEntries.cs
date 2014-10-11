using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class ConfirmBusyTimeEntries : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;

        public ConfirmBusyTimeEntries(string companyId, string employeeId, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
        }
    }
}
