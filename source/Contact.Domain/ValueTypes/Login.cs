namespace Contact.Domain.ValueTypes
{
    public class Login
    {
        public readonly string Provider;
        public readonly string Email;

        public Login(string provider, string email)
        {
            Provider = provider;
            Email = email;
        }
    }
}