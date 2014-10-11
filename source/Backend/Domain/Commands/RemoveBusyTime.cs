using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class RemoveBusyTime : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly string BusyTimeId;

        public RemoveBusyTime(string companyId, string employeeId, string busyTimeId, DateTime created, Person createdBy, string correlationId, int basedOnVersion) : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            BusyTimeId = busyTimeId;
        }
    }
}
