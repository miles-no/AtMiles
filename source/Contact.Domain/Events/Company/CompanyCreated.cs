namespace Contact.Domain.Events.Company
{
    public class CompanyCreated : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }

        public CompanyCreated(string companyId, string companyName)
        {
            CompanyId = companyId;
            CompanyName = companyName;
        }
    }
}
