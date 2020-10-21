using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CosmosStarter.Entities;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CosmosStarter
{
    public class CosmosDriver
    {
        private readonly CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        
        private readonly string _databaseId = "thetaDb";
        private readonly string _containerId = "CustomerContainer";

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        public CosmosDriver()
        {
            _cosmosClient = CosmosDbConnection.Instance;
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
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKeyPath = "/CustomerId",
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };


            this._container = await this._database.CreateContainerIfNotExistsAsync(containerProperties);
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

        public async Task AddCustomerStream(Customer customer)
        {
            try
            {
                MemoryStream streamPayload = new MemoryStream();
                await using (StreamWriter streamWriter = new StreamWriter(streamPayload, Encoding.Default, 1024, true))
                {
                    using (JsonWriter writer = new JsonTextWriter(streamWriter))
                    {
                        writer.Formatting = Newtonsoft.Json.Formatting.None;
                        Serializer.Serialize(writer, customer);
                        writer.Flush();
                        streamWriter.Flush();
                    }
                }

                streamPayload.Position = 0;
                Customer streamResponse;
                await using (Stream stream = streamPayload )
                {
                    using (ResponseMessage responseMessage = await _container.CreateItemStreamAsync(stream, new PartitionKey(customer.CustomerId)))
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            await using (stream)
                            {
                                if (typeof(Stream).IsAssignableFrom(typeof(Customer)))
                                {
                                    streamResponse  = (object)stream as Customer;
                                }

                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                                    {
                                        streamResponse  = Serializer.Deserialize<Customer>(jsonTextReader);
                                    }
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine($"Create item from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }
                }
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

