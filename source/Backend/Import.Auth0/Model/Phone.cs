using Newtonsoft.Json;

namespace Import.Auth0.Model
{
    public class Phone
    {

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}