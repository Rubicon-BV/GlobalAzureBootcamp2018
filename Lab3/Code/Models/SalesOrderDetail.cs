namespace SimpleEchoBot.Models
{
    using Newtonsoft.Json;
    using System;

    [Serializable]
    [JsonObject]
    public class SalesOrderDetail
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("productid")]
        public Guid? ProductId { get; set; }

        [JsonProperty("salesorderid")]
        public Guid? SalesOrderId { get; set; }

        [JsonProperty("uomid")]
        public Guid? UomId { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("productname")]
        public string ProductName { get; set; }

        [JsonProperty("baseamount")]
        public string Baseamount { get; set; }

        [JsonProperty("boughton")]
        public DateTime? Boughton { get; set; }

        public override string ToString()
        {
            return $"{this.ProductName}{(this.Boughton.HasValue?$" ({this.Boughton.Value.Year})":"")}";
        }
    }
}