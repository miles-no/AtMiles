using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class UpdateBusyTimeSetEndDate : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly string BusyTimeId;
        public readonly DateTime? NewEnd;

        public UpdateBusyTimeSetEndDate(string companyId, string employeeId, string busyTimeId,
            DateTime? newEnd, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            BusyTimeId = busyTimeId;
            NewEnd = newEnd;
        }
    }
}
