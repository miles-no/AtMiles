using Newtonsoft.Json;

namespace Import.Auth0.Model
{

    namespace Model
    {
        public class Email
        {

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("primary")]
            public bool Primary { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }

}
