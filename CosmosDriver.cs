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
        private Container _customerContainer, _orderContainer;



        private const string DatabaseId = "thetaDb";
        private const string CustomerContainerId = "CustomerContainer";
        private const string OrderContainerId = "OrderContainer";

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        public CosmosDriver()
        {
            _cosmosClient = CosmosDbConnection.Instance;
        }
        public async Task CreateDatabaseAsync()
        {
            try
            {
                this._database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task CreateContainersAsync()
        {
            var customerContainerProperties = new ContainerProperties()
            {
                Id = CustomerContainerId,
                PartitionKeyPath = "/CustomerId",
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };

            var orderContainerProperties = new ContainerProperties()
            {
                Id = OrderContainerId,
                PartitionKeyPath = "/CustomerId",
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };


            this._customerContainer = await this._database.CreateContainerIfNotExistsAsync(customerContainerProperties);
            this._orderContainer = await this._database.CreateContainerIfNotExistsAsync(orderContainerProperties);
        }

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                ItemResponse<Customer> customerResponse = await this._customerContainer.CreateItemAsync<Customer>(customer, new PartitionKey(customer.CustomerId));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", customerResponse.Resource.CustomerId, customerResponse.RequestCharge);
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine("Exception occured in AddCustomer: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw;
            }
        }

        #region insert-data
        public async Task AddCustomerStream(Customer customer)
        {
            try
            {
                var streamPayload = new MemoryStream();
                await using (var streamWriter = new StreamWriter(streamPayload, Encoding.Default, 1024, true))
                {
                    using (JsonWriter writer = new JsonTextWriter(streamWriter))
                    {
                        writer.Formatting = Formatting.None;
                        Serializer.Serialize(writer, customer);
                        await writer.FlushAsync();
                        await streamWriter.FlushAsync();
                    }
                }

                streamPayload.Position = 0;
                await using (Stream stream = streamPayload )
                {
                    using (ResponseMessage responseMessage = await _customerContainer.CreateItemStreamAsync(stream, new PartitionKey(customer.CustomerId)))
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            await using (stream)
                            {
                                if (typeof(Stream).IsAssignableFrom(typeof(Customer)))
                                {
                                }

                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                                    {
                                        Serializer.Deserialize<Customer>(jsonTextReader);
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


        public async Task AddOrders(List<Order> orders, string orderId)
        {
            try
            {
                foreach (var order in orders)
                {
                    ItemResponse<Order> orderResponse = await this._orderContainer.CreateItemAsync<Order>(order, new PartitionKey(orderId));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", orderResponse.Resource.CustomerId, orderResponse.RequestCharge);
                }
                
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine("Exception occured in AddOrders: {0} Message body is {1}.\n", ex.Message,ex.ResponseBody);
                throw;
            }

        }
        #endregion
        public async Task<Customer> GetCustomer(string customerId)
        {
            try
            {
                var customerResponse = await this._customerContainer.ReadItemAsync<Customer>(customerId, new PartitionKey(customerId));
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
                var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@customerid", customerId);
                FeedIterator<Order> queryResultSetIterator = this._orderContainer.GetItemQueryIterator<Order>(queryDefinition);

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

        public async Task<Order> UpdateOrder(string orderId)
        {

        }

        public async Task DeleteOrder(string orderId)
        {

        }
    }
}

