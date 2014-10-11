using no.miles.at.Backend.Domain.Services;

namespace no.miles.at.Backend.Domain.ValueTypes
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

        public static Login CreateFromSubject(string subject)
        {
            var splitted = IdService.SplitLoginSubject(subject);
            var provider = splitted.Item1;
            var email = splitted.Item2;
            return new Login(provider, email);
        }
    }
}