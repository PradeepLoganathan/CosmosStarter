using Bogus;
using CosmosStarter.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace CosmosStarter
{
    class DataGenerator
    {
        public Customer SeedCustomerData(List<Order> orders)
        {
            var Orderids = orders.Select(o => o.OrderId).ToList();

            var addressFaker = new Faker<Address>()
                .RuleFor(a => a.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.County, f => f.Address.County())
                .RuleFor(a => a.ZipCode, f => f.Address.ZipCode());

            var contactFaker = new Faker<Contact>()
                .RuleFor(o => o.Phone, f => f.Person.Phone)
                .RuleFor(o => o.EmailAddress, (f, u) => f.Internet.Email());

            var customerFaker = new Faker<Customer>()
                .CustomInstantiator(f => new Customer(new Randomizer().Replace("CU#-##-####")))
                .RuleFor(customer => customer.ModifiedDate, f => f.Date.Recent(100))
                .RuleFor(customer => customer.FirstName, f => f.Name.FirstName())
                .RuleFor(customer => customer.LastName, f => f.Name.LastName())
                .RuleFor(customer => customer.Title, f => f.Name.Prefix(f.Person.Gender))
                .RuleFor(customer => customer.Suffix, f => f.Name.Suffix())
                .RuleFor(customer => customer.MiddleName, f => f.Name.FirstName())
                .RuleFor(customer => customer.SalesPerson, f => f.Name.FullName())
                .RuleFor(customer => customer.CompanyName, f => f.Company.CompanyName())
                .RuleFor(customer => customer.CreditLimit, f => f.Finance.Amount(100, 1000))
                .RuleFor(customer => customer.Address, f => addressFaker.Generate())
                .RuleFor(customer => customer.Contact, f => contactFaker.Generate());



            var customer =  customerFaker.Generate();
            customer.OrderIDs = Orderids;

            return customer;
        }

        public List<Order> SeedOrderData(int recordCount)
        {
            var systems = new[] { "XPS", "Precision", "Optiplex", "Alienware", "Latitude" };
            List<Order> orders = new List<Order>();

            for (int i = 1; i <= recordCount; i++)
            {
                var random = RandomNumberGenerator.Create();
                var bytes = new byte[sizeof(int)]; // 4 bytes
                random.GetNonZeroBytes(bytes);
                var result = BitConverter.ToInt32(bytes);

              
                var orderFaker = new Faker<Order>()
                    .RuleFor(o => o.OrderId, new Randomizer(result).Replace("OD#-###-###"))
                    .RuleFor(o => o.Date, f => f.Date.Past(3))
                    .RuleFor(o => o.OrderValue, f => f.Finance.Amount(0, 10000))
                    .RuleFor(o => o.Config, f => f.PickRandom<string>(systems))
                    .RuleFor(order => order.Shipped, f => f.Random.Bool(0.9f));

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
    }
}
