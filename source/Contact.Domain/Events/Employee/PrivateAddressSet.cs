using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class PrivateAddressSet : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;
        public readonly Address PrivateAddress;

        public PrivateAddressSet(string companyId, string companyName, string employeeId, string employeeName, Address privateAddress, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            PrivateAddress = privateAddress;
        }
    }
}
