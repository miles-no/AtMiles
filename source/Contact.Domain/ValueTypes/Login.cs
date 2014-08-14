namespace Contact.Domain.ValueTypes
{
    public class Login
    {
        public Login(string provider, string email, string id)
        {
            Provider = provider;
            Email = email;
            Id = id;
        }

        public string Provider { get; private set; }
        public string Email { get; private set; }
        public string Id { get; private  set; }
    }
}