using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class SetPrivateAddress : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly Address PrivateAddress;

        public SetPrivateAddress(string companyId, string employeeId, Address privateAddress, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            PrivateAddress = privateAddress;
        }
    }
}
