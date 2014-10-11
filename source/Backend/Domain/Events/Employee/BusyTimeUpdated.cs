using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Employee
{
    public class BusyTimeUpdated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly string BusyTimeId;
        public readonly DateTime Start;
        public readonly DateTime? End;
        public readonly short PercentageOccpied;
        public readonly string Comment;

        public BusyTimeUpdated(string companyId, string companyName, string employeeId, string employeeName, string busyTimeId, DateTime start, DateTime? end, short percentageOccpied, string comment, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            BusyTimeId = busyTimeId;
            Start = start;
            End = end;
            PercentageOccpied = percentageOccpied;
            Comment = comment;
        }
    }
}
