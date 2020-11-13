using System;
using System.Text.Json.Serialization;

namespace CosmosStarter.Entities
{

    public class Customer
    {
        public Customer(string customerId)
        {
            CustomerId = customerId;
        }

        [JsonPropertyName("id")]
        public string CustomerNumber { get; set; }
        public string CustomerId { get; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; }
       
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public decimal CreditLimit { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
    }
}
