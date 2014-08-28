using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class MoveEmployeeToNewOffice : Command
    {
        public readonly string CompanyId;
        public readonly string OldOfficeId;
        public readonly string NewOfficeId;
        public readonly string EmployeeId;
        public MoveEmployeeToNewOffice(string companyId, string oldOfficeId, string newOfficeId, string employeeId, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OldOfficeId = oldOfficeId;
            NewOfficeId = newOfficeId;
            EmployeeId = employeeId;
        }
    }
}
