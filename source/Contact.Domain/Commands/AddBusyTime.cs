using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddBusyTime : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
        public readonly string EmployeeId;
        public readonly DateTime Start;
        public readonly DateTime? End;
        public readonly short PercentageOccpied;
        public readonly string Comment;

        public AddBusyTime(string companyId, string officeId, string employeeId,  DateTime start, DateTime? end, short percentageOccpied, string comment, DateTime created, Person createdBy, string correlationId, int basedOnVersion) : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            EmployeeId = employeeId;
            Start = start;
            End = end;
            PercentageOccpied = percentageOccpied;
            Comment = comment;
        }
    }
}
