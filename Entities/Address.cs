﻿namespace CosmosStarter.Entities
{
    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Country => "USA";

    }
}
