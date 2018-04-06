using Newtonsoft.Json;

namespace SimpleEchoBot.Models
{
    [System.Serializable]
    [JsonObject]
    public class Contact
    {
        [JsonProperty("firstname")]
        public int Firstname { get; set; }

        [JsonProperty("contactid")]
        public int ContactId { get; set; }
    }
}