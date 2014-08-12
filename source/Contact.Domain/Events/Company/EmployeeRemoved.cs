﻿namespace Contact.Domain.Events.Company
{

    public class EmployeeRemoved : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }
        public string OfficeName { get; private set; }
        public string Id { get; private set; }
        public string Name { get; private set; }

        public EmployeeRemoved(string companyId, string companyName, string officeId, string officeName, string id, string name)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            Id = id;
            Name = name;
        }
    }
}