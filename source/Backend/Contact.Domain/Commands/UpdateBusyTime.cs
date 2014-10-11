﻿using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class UpdateBusyTime : Command
    {
        public readonly string CompanyId;
        public readonly string EmployeeId;
        public readonly string BusyTimeId;
        public readonly DateTime Start;
        public readonly DateTime? End;
        public readonly short PercentageOccpied;
        public readonly string Comment;

        public UpdateBusyTime(string companyId, string employeeId, string busyTimeId,
            DateTime start, DateTime? end, short percentageOccpied, string comment,
            DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            BusyTimeId = busyTimeId;
            Start = start;
            End = end;
            PercentageOccpied = percentageOccpied;
            Comment = comment;
        }
    }
}
