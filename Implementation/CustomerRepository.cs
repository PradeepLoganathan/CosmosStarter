using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Cosmos;
using CosmosStarter.Entities;
using CosmosStarter.Interfaces;

namespace CosmosStarter.Implementation
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

        public async Task<List<Customer>> GetCustomersWithHighCreditLimit()
        {
            var queryDefinition = new QueryDefinition("select * from c where c.value > @creditlimit")
                .WithParameter("@creditlimit", 1000);
            var resultSet = this._cosmosDbContext.CustomerContainer.GetItemQueryIterator<Customer>(
                queryDefinition,null,
                new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey("CustomerId"),
                    MaxItemCount = 1
                });

            var customers = new List<Customer>();
            await foreach (var result in resultSet)
                customers.Add(result);

            return customers;

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
