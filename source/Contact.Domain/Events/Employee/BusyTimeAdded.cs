using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class BusyTimeAdded : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly string BusyTimeId;
        public readonly DateTime Start;
        public readonly DateTime? End;
        public readonly short PercentageOccpied;
        public readonly string Comment;

        public BusyTimeAdded(string companyId, string companyName, string officeId, string officeName, string employeeId, string employeeName, string busyTimeId, DateTime start, DateTime? end, short percentageOccpied, string comment, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
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
