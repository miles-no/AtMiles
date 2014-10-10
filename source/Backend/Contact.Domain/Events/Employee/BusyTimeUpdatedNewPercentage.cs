using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class BusyTimeUpdatedNewPercentage : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly string BusyTimeId;
        public readonly short NewPercentageOccpied;

        public BusyTimeUpdatedNewPercentage(string companyId, string companyName, string employeeId, string employeeName, string busyTimeId, short newPercentageOccpied, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            BusyTimeId = busyTimeId;
            NewPercentageOccpied = newPercentageOccpied;
        }
    }
}
