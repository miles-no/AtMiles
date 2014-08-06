using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class OpenOffice : Command
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }

        public OpenOffice(string name)
        {
            Name = name;
            Address = null;
        }

        public OpenOffice WithAddress(Address address)
        {
            Address = address;
            return this;
        }
    }
}
