using System;

namespace no.miles.at.Backend.Domain.Services
{
    public class IdService
    {
        public static string CreateNewId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", string.Empty)
                .Replace("/", string.Empty)
                .Replace(":", string.Empty)
                .Replace("&", string.Empty)
                .Replace("%", string.Empty)
                .Replace("?", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("+", string.Empty);
        }

        public static Tuple<string, string> SplitLoginSubject(string subject)
        {
            var provider = string.Empty;
            var email = string.Empty;
            if (!string.IsNullOrEmpty(subject))
            {
                var parts = subject.Split('|');
                if (parts.Length == 2)
                {
                    provider = parts[0];
                    email = parts[1];
                }
            }
            return new Tuple<string, string>(provider, email);
        }
    }
}
