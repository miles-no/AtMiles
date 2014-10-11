using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Infrastructure
{
    public interface ICreateUser
    {
        void CreateUser(string companyId, string companyName, string employeeId, Login loginId, string firstName, string middleName, string lastName);
    }
}
