using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Import.Auth0.Model;
using Newtonsoft.Json;

namespace Import.Auth0
{
    public class GetUsersFromAuth0 : IGetUsersFromAuth0
    {
        private readonly string token;

        public GetUsersFromAuth0(string token)
        {
            this.token = token;
        }

        public async Task<Auth0User> GetUser(string email)
        {
            return (await GetFromAuth0(email)).FirstOrDefault();
        }

        public async Task<List<Auth0User>> GetUsers()
        {
            return await GetFromAuth0();
        }

        private async Task<List<Auth0User>> GetFromAuth0(string email = null)
        {
            List<Auth0User> res;
            var client = new WebClient();

            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
            client.Headers[HttpRequestHeader.Accept] = "application/json";

            var url = "https://atmiles.auth0.com/api/users";
            
            if (string.IsNullOrEmpty(email) == false)
            {
                url += "?search=" + email;
            }
            
            var resString = await client.DownloadStringTaskAsync(new Uri(url));
            res = JsonConvert.DeserializeObject<List<Auth0User>>(resString);

            return res;
        }

        
    }
}