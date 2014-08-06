namespace Contact.Domain.Events
{
    public class OfficeClosed : Event
    {
        public string OfficeId { get; private set; }

        public string Name { get; private set; }

        public OfficeClosed(string officeId, string name)
        {
            OfficeId = officeId;
            Name = name;
        }
    }
}
