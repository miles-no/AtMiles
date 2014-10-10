namespace Contact.Domain.ValueTypes
{
    public class EmployeeLoginInfo
    {
        public readonly string Id;
        public readonly Login LoginId;

        public EmployeeLoginInfo(string id, Login loginId)
        {
            Id = id;
            LoginId = loginId;
        }
    }
}
