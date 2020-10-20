using System;
using System.Collections.Generic;
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
                Console.WriteLine("Exception occured in AddCustomer: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw;
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
                Console.WriteLine("Exception occured in AddOrders: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw;
            }

        }

        public async Task<Customer> GetCustomer(string customerId)
        {
            try
            {
                var customerResponse = await this._container.ReadItemAsync<Customer>(customerId, new PartitionKey(customerId));
                return customerResponse;
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine("Exception occured in GetCustomer: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw ;
            }

        }

        public async Task<List<Order>> GetOrders(string customerId)
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c WHERE c.CustomerId = @customerid";

                Console.WriteLine("Running query: {0}\n", sqlQueryText);
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@customerid", "CU7-36-8183" );
                FeedIterator<Order> queryResultSetIterator = this._container.GetItemQueryIterator<Order>(queryDefinition);

                List<Order> orders = new List<Order>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Order> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Order order in currentResultSet)
                    {
                        orders.Add(order);
                        Console.WriteLine("\tRead {0}\n", order.OrderId);
                    }
                }

                return orders;
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine("Exception occured in GetCustomer: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw ;
            }

        }
    }
}

