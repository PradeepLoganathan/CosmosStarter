using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace CosmosStarter.Interfaces
{
    public interface ICosmosDBContext
    {
        Container CustomerContainer { get; }
        Container OrdersContainer { get; }

        Task CreateDatabaseAsync();
        Task CreateContainersAsync();
        
        Task Initialize();

        Task DeleteContainers();
        Task DeleteDatabase();
    }
}