namespace Contact.Domain.Services
{
    public class NameService
    {
        public static string GetName(string firstName, string lastName)
        {
            return GetName(firstName, string.Empty, lastName);
        }

        public static string GetName(string firstName, string middleName, string lastName)
        {
            string name = firstName;
            
            if (!string.IsNullOrEmpty(middleName))
            {
                name += " " + middleName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                name += " " + lastName;
            }

            return name;
        }
    }
}
