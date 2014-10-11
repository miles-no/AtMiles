using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Employee
{
    public class BusyTimeConfirmed : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;

        public BusyTimeConfirmed(string companyId, string companyName, string employeeId, string employeeName, DateTime created, Person createdBy, string correlationId) : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
        }
    }
}
