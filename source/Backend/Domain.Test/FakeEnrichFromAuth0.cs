using System.Collections.Generic;
using System.Threading.Tasks;
using Import.Auth0;
using Import.Auth0.Model;

namespace no.miles.at.Backend.Domain.Test
{
    public class FakeEnrichFromAuth0 : IGetUsersFromAuth0
    {
        public async Task<List<Auth0User>> GetUsers()
        {
            return await Task.Run(() => new List<Auth0User>());
        }
    }
}