namespace Contact.Domain.Commands
{
    public class AddCompanyAdmin : Command
    {
        public string NewAdminId { get; private set; }

        public AddCompanyAdmin(string newAdminId)
        {
            NewAdminId = newAdminId;
        }
    }
}
