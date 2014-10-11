namespace Contact.Domain.ValueTypes
{
    public class SimpleUserInfo
    {
        public readonly string Id;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;
        public readonly Login LoginId;

        public SimpleUserInfo(string id, string firstName, string middleName, string lastName, Login loginId)
        {
            Id = id;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            LoginId = loginId;
        }
    }
}
