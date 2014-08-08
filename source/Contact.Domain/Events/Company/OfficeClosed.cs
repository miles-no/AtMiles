namespace Contact.Domain.Events.Company
{
    public class OfficeClosed : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }

        public string OfficeName { get; private set; }

        public OfficeClosed(string companyId, string companyName, string officeId, string officeName)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
        }
    }
}
