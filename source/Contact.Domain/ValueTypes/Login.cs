using Contact.Domain.Services;

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

        public Login(string subject)
        {
            var splitted = IdService.SplitLoginSubject(subject);
            Provider = splitted.Item1;
            Email = splitted.Item2;
        }
    }
}