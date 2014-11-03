using System.Collections.Generic;
using System.Threading.Tasks;
using Import.Auth0.Model;

namespace Import.Auth0
{
    public interface IGetUsersFromAuth0
    {
        Task<Auth0User> GetUser(string email);
        Task<List<Auth0User>> GetUsers();
    }
}