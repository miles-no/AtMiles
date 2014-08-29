namespace Contact.Domain.ValueTypes
{
    public class Login
    {
        public readonly string Provider;
        public readonly string Email;
        public readonly string Id;

        public Login(string provider, string email, string id)
        {
            Provider = provider;
            Email = email;
            Id = id;
        }
    }
}