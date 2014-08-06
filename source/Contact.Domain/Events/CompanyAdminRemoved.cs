namespace Contact.Domain.Events
{
    public class CompanyAdminRemoved : Event
    {
        public string AdminId { get; private set; }
        public string Name { get; private set; }

        public CompanyAdminRemoved(string adminId, string name)
        {
            AdminId = adminId;
            Name = name;
        }
    }
}
