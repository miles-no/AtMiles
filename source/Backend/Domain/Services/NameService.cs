namespace no.miles.at.Backend.Domain.Services
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
            if (name == null)
            {
                name = string.Empty;
            }
            
            if (!string.IsNullOrEmpty(middleName))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    name += " ";
                }

                name += middleName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    name += " ";
                }
                name += lastName;
            }

            return name;
        }
    }
}
