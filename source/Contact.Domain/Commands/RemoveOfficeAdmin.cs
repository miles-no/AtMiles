namespace Contact.Domain.Commands
{
    public class RemoveOfficeAdmin : Command
    {
        public string CompanyId { get; private set; }
        public string OfficeId { get; private set; }
        public string AdminId { get; private set; }
        

        public RemoveOfficeAdmin(string companyId, string officeId, string adminId)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            AdminId = adminId;
        }
    }
}
