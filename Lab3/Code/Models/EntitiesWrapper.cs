using Newtonsoft.Json;

namespace SimpleEchoBot.Models
{
    public class Entities
    {
        [JsonObject]
        public class EntitiesWrapper<T>
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
            public string PagingCookie { get; set; }

            [JsonProperty("value")]
            public T[] Entities { get; set; }

            [JsonProperty("@odata.nextLink")]
            public string NextLink { get; set; }
        }
    }
}