using Contact.Domain.ValueTypes;

namespace Contact.Infrastructure
{
    public interface ICreateUser
    {
        void CreateUser(string companyId, string companyName, string employeeId, Login loginId, string firstName, string middleName, string lastName);
    }
}
