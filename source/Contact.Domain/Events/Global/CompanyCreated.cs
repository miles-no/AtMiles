namespace Contact.Domain.Events.Global
{
    public class CompanyCreated : Event
    {
        public readonly string Id;
        public readonly string Name;

        public CompanyCreated(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
