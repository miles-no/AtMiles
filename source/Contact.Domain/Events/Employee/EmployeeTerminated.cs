using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class EmployeeTerminated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;

        public EmployeeTerminated(string companyId, string companyName, string officeId, string officeName, string employeeId, string employeeName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
        }
    }
}
