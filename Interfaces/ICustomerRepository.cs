using CosmosStarter.Entities;
using System.Threading.Tasks;

namespace CosmosStarter.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomer(Customer customer);
        Task DeleteCustomer(string customerId);
        Task<Customer> GetCustomer(string customerId);
        Task UpdateCustomer(Customer customer);
    }
}