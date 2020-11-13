using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using CosmosStarter.Seeders;

namespace CosmosStarter.Implementation
{
    public class CosmosDriver : ICosmosDriver
    {

        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public CosmosDriver(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            this._orderRepository = orderRepository;
            this._customerRepository = customerRepository;
        }

      
        public async Task Drive()
        {
            var (customer, orders) = SeedData();
            await _customerRepository.AddCustomer(customer);
            await _orderRepository.AddOrders(orders, customer.CustomerId);
            var cust = await _customerRepository.GetCustomer("CU8-75-6837");
            
        }


        private static (Customer, List<Order>) SeedData()
        {
            var orders = DataGenerator.SeedOrderData(50);
            var customer = DataGenerator.SeedCustomerData();
            DataGenerator.AddOrdersToCustomer(orders, customer.CustomerId);

            return (customer, orders);

            //await dataGenerator.SerializeCustomerData(customer);
            //await dataGenerator.SerializeOrderData(orders);
        }


    }
}