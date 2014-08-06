namespace Contact.Domain.Commands
{
    public class RemoveCompanyAdmin : Command
    {
        public string AdminId { get; private set; }

        public RemoveCompanyAdmin(string adminId)
        {
            AdminId = adminId;
        }
    }
}
