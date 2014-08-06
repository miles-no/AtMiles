namespace Contact.Domain.Commands
{
    public class CloseOffice : Command
    {
        public string OfficeId { get; private set; }

        public CloseOffice(string officeId)
        {
            OfficeId = officeId;
        }
    }
}
