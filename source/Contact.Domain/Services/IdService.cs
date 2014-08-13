using System;

namespace Contact.Domain.Services
{
    public class IdService
    {
        public static string CreateNewId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", string.Empty);
        }
    }
}
