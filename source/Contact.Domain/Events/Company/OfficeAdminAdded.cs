using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Company
{
    public class OfficeAdminAdded : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }
        public string OfficeName { get; private set; }
        public string AdminId { get; private set; }
        public string AdminName { get; private set; }



        public OfficeAdminAdded(string companyId, string companyName, string officeId, string officeName, string adminId, string adminName, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
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
