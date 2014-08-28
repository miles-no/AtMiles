using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class EmployeeMovedToNewOffice : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OldOfficeId;
        public readonly string OldOfficeName;
        public readonly string NewOfficeId;
        public readonly string NewOfficeName;
        public readonly string EmployeeId;
        public readonly string EmployeeName;

        public EmployeeMovedToNewOffice(string companyId, string companyName, string oldOfficeId, string oldOfficeName, string newOfficeId, string newOfficeName, string employeeId, string employeeName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OldOfficeId = oldOfficeId;
            OldOfficeName = oldOfficeName;
            NewOfficeId = newOfficeId;
            NewOfficeName = newOfficeName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
        }
    }
}
