using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CosmosStarter.Entities
{

    public class Customer
    {
        public Customer(string customerID)
        {
            CustomerId = customerID;
        }

        [JsonProperty(PropertyName = "id")]
        public string CustomerId { get; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public decimal CreditLimit { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public IEnumerable<string> OrderIDs { get; set; }
    }
}
