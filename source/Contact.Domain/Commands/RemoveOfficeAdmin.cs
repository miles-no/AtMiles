namespace Contact.Domain.Commands
{
    public class RemoveOfficeAdmin : Command
    {
        public string AdminId { get; private set; }
        public string OfficeId { get; private set; }

        public RemoveOfficeAdmin(string adminId, string officeId)
        {
            AdminId = adminId;
            OfficeId = officeId;
        }
    }
}
