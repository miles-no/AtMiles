using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class BusyTimeUpdatedNewEndDate : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly string BusyTimeId;
        public readonly DateTime? NewEnd;

        public BusyTimeUpdatedNewEndDate(string companyId, string companyName, string employeeId, string employeeName, string busyTimeId, DateTime? newEnd, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            BusyTimeId = busyTimeId;
            NewEnd = newEnd;
        }
    }
}
