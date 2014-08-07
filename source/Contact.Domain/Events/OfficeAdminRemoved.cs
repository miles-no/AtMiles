﻿namespace Contact.Domain.Events
{
    public class OfficeAdminRemoved : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }
        public string OfficeName { get; private set; }
        public string AdminId { get; private set; }
        public string AdminName { get; private set; }

        public OfficeAdminRemoved(string companyId, string companyName, string officeId, string officeName, string adminId, string adminName)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            AdminId = adminId;
            AdminName = adminName;
        }
    }
}
