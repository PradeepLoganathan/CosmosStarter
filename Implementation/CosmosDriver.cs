using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using Newtonsoft.Json;

namespace CosmosStarter
{
    public class CosmosDriver : ICosmosDriver
    {

        private readonly IOrderRepository orderRepository;
        private readonly ICustomerRepository customerRepository;

        public CosmosDriver(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            this.orderRepository = orderRepository;
            this.customerRepository = customerRepository;
        }

      
        public async Task Drive()
        {
            var data = SeedData();
            await customerRepository.AddCustomer(data.Item1);
            await orderRepository.AddOrders(data.Item2, data.Item1.CustomerId);
        }


        private (Customer, List<Order>) SeedData()
        {
            var Serializer = new JsonSerializer();
            var dataGenerator = new DataGenerator();
            var orders = dataGenerator.SeedOrderData(10);
            var customer = dataGenerator.SeedCustomerData(orders);
            dataGenerator.AddOrdersToCustomer(orders, customer.CustomerId);

            return (customer, orders);

            //await dataGenerator.SerializeCustomerData(customer);
            //await dataGenerator.SerializeOrderData(orders);
        }


    }
}