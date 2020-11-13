using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    public class DataGenerator
    {
        public static Customer SeedCustomerData()
        {
            var customerFaker = new CustomerFaker();
            var customer = customerFaker.Generate();
            customer.CustomerNumber = customer.CustomerId;
            return customer;
        }

        public static List<Order> SeedOrderData(int recordCount)
        {
            var orderFaker = new OrderFaker();
            var orders = orderFaker.Generate(recordCount);
            foreach (var order in orders)
            {
                order.OrderId = Guid.NewGuid().ToString();
            }
            return orders;
        }

        public async Task SerializeCustomerData(Customer customer)
        {
            using (FileStream fs = File.Create("./customer.json"))
            {
                await JsonSerializer.SerializeAsync(fs, customer);
            }
        }

        public async Task SerializeOrderData(List<Order> orders)
        {
            foreach (var order in orders)
            {
                var fileName = "./" + order.OrderId + ".json";
                using (FileStream fs = File.Create(fileName))
                {
                    await JsonSerializer.SerializeAsync(fs, order);
                }
            }

        }

        public static void AddOrdersToCustomer(IEnumerable<Order> orders, string customerId)
        {
            foreach (var order in orders)
            {
                order.CustomerId = customerId;
            }
        }
    }
}
