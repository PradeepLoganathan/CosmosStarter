using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;
using Azure.Cosmos;

namespace CosmosStarter
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICosmosDbContext _cosmosDbContext;
        public CustomerRepository(ICosmosDbContext cosmosDBContext)
        {
            this._cosmosDbContext = cosmosDBContext;
        }

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                ItemResponse<Customer> customerResponse = await this._cosmosDbContext.CustomerContainer.CreateItemAsync<Customer>(customer, new PartitionKey(customer.CustomerId));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine("Exception occured in AddCustomer: Message body is {0}.\n", ex.Message);
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
                Console.WriteLine("Exception occured in GetCustomer: Message body is {0}.\n", ex.Message );
                throw;
            }
        }

    }
}
