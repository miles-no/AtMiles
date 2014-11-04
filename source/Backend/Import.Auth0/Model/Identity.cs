using Newtonsoft.Json;

namespace Import.Auth0.Model
{
    public class Identity
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("connection")]
        public string Connection { get; set; }

        [JsonProperty("isSocial")]
        public bool IsSocial { get; set; }
    }
}