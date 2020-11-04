using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Cosmos;

namespace CosmosStarter
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
                foreach (var order in orders)
                {
                    ItemResponse<Order> orderResponse = await this._cosmosDbContext.OrdersContainer.CreateItemAsync<Order>(order, new PartitionKey(customerId));
                }

            }
            catch (CosmosException ex)
            {
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
                    queryDefinition,null,
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
