using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class RemoveBusyTime : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
        public readonly string EmployeeId;
        public readonly string BusyTimeId;

        public RemoveBusyTime(string companyId, string officeId, string employeeId, string busyTimeId, DateTime created, Person createdBy, string correlationId, int basedOnVersion) : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            EmployeeId = employeeId;
            BusyTimeId = busyTimeId;
        }
    }
}
