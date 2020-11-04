using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using CosmosStarter.Entities;

namespace CosmosStarter.Seeders
{
    class DataGenerator
    {
        public Customer SeedCustomerData()
        {
            //var Orderids = orders.Select(o => o.OrderId).ToList();
            var customerFaker = new CustomerFaker();
            var customer = customerFaker.Generate();
            customer.CustomerNumber = customer.CustomerId;
            return customer;
        }

        public List<Order> SeedOrderData(int recordCount)
        {

            List<Order> orders = new List<Order>();

            for (int i = 1; i <= recordCount; i++)
            {
                var orderFaker = new OrderFaker();
                var order = orderFaker.Generate();
                orders.Add(order);
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

        public void AddOrdersToCustomer(IEnumerable<Order> orders, string customerId)
        {
            foreach (var order in orders)
            {
                order.CustomerId = customerId;
            }
        }
    }
}
