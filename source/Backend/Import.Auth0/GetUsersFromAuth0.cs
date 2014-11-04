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
        private readonly string clientId;
        private readonly string clientSecret;

        // https://atmiles.auth0.com
        private readonly string url;
        private string token;

        public GetUsersFromAuth0(string clientId, string clientSecret, string url)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.url = url;
        }

        public async Task<List<Auth0User>> GetUsers()
        {
            token = await GetToken();
            return await GetFromAuth0();
        }

        public class TokenResult
        {
            public string access_token { get; set; }
        }

        private async Task<string> GetToken()
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            var request = JsonConvert.SerializeObject(
                new
                {
                    client_id = clientId, client_secret = clientSecret, grant_type = "client_credentials"
                });
            
            var response = await client.UploadStringTaskAsync(url + "/oauth/token", request);
            
            var res =JsonConvert.DeserializeObject<TokenResult>(response);

            return res.access_token;
        }

        private async Task<List<Auth0User>> GetFromAuth0()
        {
            List<Auth0User> res;
            var client = new WebClient();

            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
            client.Headers[HttpRequestHeader.Accept] = "application/json";

            var resString = await client.DownloadStringTaskAsync(new Uri(url + "/api/users"));
            res = JsonConvert.DeserializeObject<List<Auth0User>>(resString);

            return res;
        }

        
    }
}