using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Employee
{
    public class DateOfBirthSet : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly DateTime DateOfBirth;

        public DateOfBirthSet(string companyId, string companyName, string employeeId, string employeeName, DateTime dateOfBirth, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            DateOfBirth = dateOfBirth;
        }
    }
}
