using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
{
    public class OfficeOpened : Event
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public Address Address { get; private set; }

        public OfficeOpened(string id, string name, Address address)
        {
            Id = id;
            Name = name;
            Address = address;
        }
    }
}
