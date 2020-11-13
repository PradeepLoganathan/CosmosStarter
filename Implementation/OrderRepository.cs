using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Cosmos;
using CosmosStarter.Entities;
using CosmosStarter.Interfaces;

namespace CosmosStarter.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ICosmosDbContext _cosmosDbContext;
        public OrderRepository(ICosmosDbContext cosmosDbContext)
        {
            this._cosmosDbContext = cosmosDbContext;
        }

        public async Task AddOrders(List<Order> orders, string customerId)
        {
            try
            {
                var ordersToInsert1 = new List<KeyValuePair<PartitionKey, Stream>>();
                var ordersToInsert = new Dictionary<PartitionKey, Stream>();
                foreach (var order in orders)
                {
                    var stream = new MemoryStream();
                    await JsonSerializer.SerializeAsync(stream, order);
                    //ordersToInsert.Add(new PartitionKey(order.CustomerId), stream); 
                    ordersToInsert1.Add(new KeyValuePair<PartitionKey, Stream>(new PartitionKey(order.CustomerId), stream)); 
                }

                var tasks = new List<Task>();

                foreach (var (key, value) in ordersToInsert1)
                {

                    value.Position = 0;
                    var jsonString = await JsonSerializer.DeserializeAsync<Order>(value);
                    Console.WriteLine(jsonString);

                    tasks.Add(this._cosmosDbContext.OrdersContainer.CreateItemStreamAsync(value, key)
                        .ContinueWith(x =>
                        {
                            var response = x.Result;
                            Console.WriteLine($"Bulk insert {response.ClientRequestId} has status {response.Status} with message {response}");
                        }));
                }
                
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task DeleteOrder(string orderId, string customerId)
        {
            try
            {
                await this._cosmosDbContext.OrdersContainer.DeleteItemAsync<Order>(orderId, new PartitionKey(customerId));
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task UpdateOrder(Order order)
        {

        }

        public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
        {
            try
            {
                const string sqlQueryText = "SELECT * FROM c WHERE c.CustomerId = @customerid";

                Console.WriteLine("Running query: {0}\n", sqlQueryText);
                var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@customerid", customerId);
                var resultSet = this._cosmosDbContext.OrdersContainer.GetItemQueryIterator<Order>(
                    queryDefinition, null,
                    new QueryRequestOptions()
                    {
                        PartitionKey = new PartitionKey("CustomerId"),
                        MaxItemCount = 1
                    });

                var orders = new List<Order>();
                await foreach (var result in resultSet)
                    orders.Add(result);

                return orders;

            }
            catch (CosmosException)
            {
                throw;
            }
        }

        public async Task<Order> GetOrder(string orderId, string customerId)
        {
            try
            {
                var orderResponse = await this._cosmosDbContext.OrdersContainer.ReadItemAsync<Order>(orderId, new PartitionKey(customerId));
                return orderResponse;
            }
            catch (CosmosException)
            {
                throw;
            }
        }
    }
}
