using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class UpdateBusyTimeChangePercentage : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly string BusyTimeId;
        public readonly short NewpercentageOccpied;

        public UpdateBusyTimeChangePercentage(string companyId, string employeeId, string busyTimeId,
            short newpercentageOccpied, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            BusyTimeId = busyTimeId;
            NewpercentageOccpied = newpercentageOccpied;
        }
    }
}
