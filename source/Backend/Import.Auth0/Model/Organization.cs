using Newtonsoft.Json;

namespace Import.Auth0.Model
{
    public class Organization
    {

        [JsonProperty("primary")]
        public bool Primary { get; set; }

        [JsonProperty("customType")]
        public string CustomType { get; set; }
    }
}