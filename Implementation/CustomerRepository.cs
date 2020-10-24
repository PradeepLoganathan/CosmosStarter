using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosStarter
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICosmosDBContext _cosmosDbContext;
        public CustomerRepository(ICosmosDBContext cosmosDBContext)
        {
            this._cosmosDbContext = cosmosDBContext;
        }

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                ItemResponse<Customer> customerResponse = await this._cosmosDbContext.CustomerContainer.CreateItemAsync<Customer>(customer, new PartitionKey(customer.CustomerId));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", customerResponse.Resource.CustomerId, customerResponse.RequestCharge);
            }
            catch (CosmosException ex)
            {
                Console.WriteLine("Exception occured in AddCustomer: {0} Message body is {1}.\n", ex.Message, ex.ResponseBody);
                throw;
            }
        }

        public async Task DeleteCustomer(string customerId)
        {

        }

        public async Task UpdateCustomer(Customer customer)
        {

        }

        public async Task<Customer> GetCustomer(string customerId)
        {
            try
            {
                var customerResponse = await this._cosmosDbContext.CustomerContainer.ReadItemAsync<Customer>(customerId, new PartitionKey(customerId));
                return customerResponse;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine("Exception occured in GetCustomer: {0} Message body is {1}.\n", ex.Message, ex.ResponseBody);
                throw;
            }
        }

    }
}
