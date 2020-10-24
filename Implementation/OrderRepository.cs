using CosmosStarter.Entities;
using CosmosStarter.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosStarter
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ICosmosDBContext cosmosDBContext;
        public OrderRepository(ICosmosDBContext cosmosDBContext)
        {
            this.cosmosDBContext = cosmosDBContext;
        }

        public async Task AddOrders(List<Order> orders, string customerId)
        {
            try
            {
                foreach (var order in orders)
                {
                    ItemResponse<Order> orderResponse = await this.cosmosDBContext.OrdersContainer.CreateItemAsync<Order>(order, new PartitionKey(customerId));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", orderResponse.Resource.CustomerId, orderResponse.RequestCharge);
                }

            }
            catch (CosmosException ex)
            {
                Console.WriteLine("Exception occured in AddOrders: {0} Message body is {1}.\n", ex.Message, ex.ResponseBody);
                throw;
            }
        }

        public async Task DeleteOrder(string orderId, string customerId)
        {
            try
            {
                await this.cosmosDBContext.OrdersContainer.DeleteItemAsync<Order>(orderId, new PartitionKey(customerId));
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
                var sqlQueryText = "SELECT * FROM c WHERE c.CustomerId = @customerid";

                Console.WriteLine("Running query: {0}\n", sqlQueryText);
                var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@customerid", customerId);
                FeedIterator<Order> queryResultSetIterator = this.cosmosDBContext.OrdersContainer.GetItemQueryIterator<Order>(queryDefinition);

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
                Console.WriteLine("Exception occured in GetCustomer: {0} Message body is {1}.\n", ex.Message, ex.ResponseBody);
                throw;
            }
        }

        public async Task<Order> GetOrder(string orderId, string customerId)
        {
            try
            {
                var orderResponse = await this.cosmosDBContext.OrdersContainer.ReadItemAsync<Order>(orderId, new PartitionKey(customerId));
                return orderResponse;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine("Exception occured in GetCustomer: {0} Message body is {1}.\n", ex.Message, ex.ResponseBody);
                throw;
            }
        }
    }
}
