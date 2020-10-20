using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CosmosStarter.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosStarter
{
    public class CosmosDriver
    {
        private readonly CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        private const string EndpointUri = "https://localhost:8081";
        private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string _databaseId = "thetaDb";
        private readonly string _containerId = "CustomerContainer";
        public CosmosDriver()
        {
            _cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        }
        public async Task CreateDatabaseAsync()
        {
            try
            {
                this._database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task CreateContainerAsync()
        {
            this._container = await this._database.CreateContainerIfNotExistsAsync(_containerId, "/CustomerId");
        }

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                ItemResponse<Customer> customerResponse = await this._container.CreateItemAsync<Customer>(customer, new PartitionKey(customer.CustomerId));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", customerResponse.Resource.CustomerId, customerResponse.RequestCharge);
            }
            catch (CosmosException ex) 
            {
                
            }

        }

        public async Task AddOrders(List<Order> orders, string CustomerId)
        {
            try
            {
                foreach (var order in orders)
                {
                    ItemResponse<Order> orderResponse = await this._container.CreateItemAsync<Order>(order, new PartitionKey(CustomerId));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", orderResponse.Resource.CustomerId, orderResponse.RequestCharge);
                }
                
            }
            catch (CosmosException ex) 
            {
                
            }

        }



        public async Task GetCustomerTask(string customerId, string companyName)
        {
            try
            {
                var customerResponse = await this._container.ReadItemAsync<Customer>(customerId, new PartitionKey(companyName));
                Console.WriteLine("Item in database with id: {0} already exists\n", customerId);
            }
            catch (CosmosException ex) 
            {
                
            }

        }
    }
}

