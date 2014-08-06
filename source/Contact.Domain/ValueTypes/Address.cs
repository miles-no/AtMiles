namespace Contact.Domain.ValueTypes
{
    public class Address
    {
        public string Street { get; private set; }
        public string PostalCode { get; private set; }
        public string PostalName { get; private set; }

        public Address(string street, string postalCode, string postalName)
        {
            Street = street;
            PostalCode = postalCode;
            PostalName = postalName;
        }
    }
}
