using System;
using Newtonsoft.Json;

namespace CosmosStarter.Entities
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Config { get; set; }
        public decimal OrderValue { get; set; }
        public bool Shipped { get; set; }
        public string CustomerId { get; set; }
    }
}
