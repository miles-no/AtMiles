namespace Contact.Domain.ValueTypes
{
    public class Address
    {
        public readonly string Street;
        public readonly string PostalCode;
        public readonly string PostalName;

        public Address(string street, string postalCode, string postalName)
        {
            Street = street;
            PostalCode = postalCode;
            PostalName = postalName;
        }
    }
}
