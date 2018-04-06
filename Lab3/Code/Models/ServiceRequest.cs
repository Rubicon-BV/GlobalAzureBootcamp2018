using Newtonsoft.Json;
using System;

namespace SimpleEchoBot.Models
{
    [Serializable]
    [JsonObject]
    public class ServiceRequest
    {
        [JsonProperty("customerid_contact@odata.bind")]
        public string JsonCustomer
        {
            get
            {
                if (this.CustomerId.HasValue)
                {
                    return $"/contacts({this.CustomerId})";
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public Guid? CustomerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime ScheduledOn { get; set; }

        [JsonProperty("followupby")]
        public string FollowUpBy
        {
            get
            {
                return this.ScheduledOn.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
        }

        [JsonProperty("casetypecode")]
        public int CaseType
        {
            get
            {
                return 1;
            }
        }
    }
}