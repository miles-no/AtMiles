namespace Contact.Domain.Commands
{
    public class CloseOffice : Command
    {
        public string CompanyId { get; private set; }
        public string OfficeId { get; private set; }

        public CloseOffice(string companyId, string officeId)
        {
            CompanyId = companyId;
            OfficeId = officeId;
        }
    }
}
