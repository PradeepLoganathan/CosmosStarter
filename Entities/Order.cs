using System;

namespace CosmosStarter.Entities
{
    public class Order
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Config { get; set; }
        public decimal OrderValue { get; set; }
        public bool Shipped { get; set; }
    }

}
