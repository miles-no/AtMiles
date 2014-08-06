namespace Contact.Domain.Events
{
    public class CompanyAdminAdded : Event
    {
        public string NewAdminId { get; private set; }
        public string Name { get; private set; }

        public CompanyAdminAdded(string newAdminId, string name)
        {
            NewAdminId = newAdminId;
            Name = name;
        }
    }
}
