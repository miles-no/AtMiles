namespace Contact.Domain.Commands
{
    public class RemoveCompanyAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string AdminId { get; private set; }

        public RemoveCompanyAdmin(string companyId, string adminId)
        {
            CompanyId = companyId;
            AdminId = adminId;
        }
    }
}
