using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class EmployeeCreated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly Login LoginId;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;

        public EmployeeCreated(string companyId, string companyName, string employeeId,
            Login loginId, string firstName, string middleName, string lastName,
            DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            LoginId = loginId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }
    }
}
