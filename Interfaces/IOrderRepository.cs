using CosmosStarter.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosStarter.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrders(List<Order> orders, string customerId);
        Task DeleteOrder(string orderId, string customerId);
        Task<List<Order>> GetOrdersByCustomerId(string customerId);
        Task<Order> GetOrder(string orderId, string customerId);
        Task UpdateOrder(Order order);
    }
}