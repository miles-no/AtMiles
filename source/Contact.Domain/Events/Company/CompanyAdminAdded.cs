﻿using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class CompanyAdminAdded : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string NewAdminId { get; private set; }
        public string NewAdminName { get; private set; }

        public CompanyAdminAdded(string companyId, string companyName, string newAdminId, string newAdminName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            NewAdminId = newAdminId;
            NewAdminName = newAdminName;
        }
    }
}
