using System.Threading.Tasks;
using Azure.Cosmos;

namespace CosmosStarter.Interfaces
{
    public interface ICosmosDbContext
    {
        CosmosContainer CustomerContainer { get; }
        CosmosContainer OrdersContainer { get; }

        Task CreateDatabaseAsync();
        Task CreateContainersAsync();
        
        Task Initialize();

        Task DeleteContainers();
        Task DeleteDatabase();
    }
}