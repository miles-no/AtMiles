namespace Contact.Domain.Commands
{
    public class AddCompanyAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string NewAdminId { get; private set; }

        public AddCompanyAdmin(string companyId, string newAdminId)
        {
            CompanyId = companyId;
            NewAdminId = newAdminId;
        }
    }
}
