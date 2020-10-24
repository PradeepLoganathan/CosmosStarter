using CosmosStarter.Configuration;
using CosmosStarter.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosStarter
{
    public class CosmosDBContext : ICosmosDBContext
    {
        private readonly CosmosConfig _cosmosConfig;
        private CosmosClient _cosmosClient;
        private Database _database;

        private const string DatabaseId = "thetaDb";
        private const string CustomerContainerId = "CustomerContainer";
        private const string OrderContainerId = "OrderContainer";

        public Container CustomerContainer { get; private set; }
        public Container OrdersContainer { get; private set; }

        public CosmosDBContext(CosmosConfig cosmosConfig)
        {
            this._cosmosConfig = cosmosConfig;
        }

        public async Task Initialize()
        {
            CosmosClientOptions CosmosClientOptions = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Direct,
                ApplicationName = "CosmosStarter",
                PortReuseMode = PortReuseMode.PrivatePortPool
            };

            _cosmosClient = new CosmosClient(_cosmosConfig.Endpoint, _cosmosConfig.Key, CosmosClientOptions);

            await CreateDatabaseAsync();
            await CreateContainersAsync();
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


            this.CustomerContainer = await this._database.CreateContainerIfNotExistsAsync(customerContainerProperties);
            this.OrdersContainer = await this._database.CreateContainerIfNotExistsAsync(orderContainerProperties);
        }

        public async Task DeleteContainers()
        {
            await CustomerContainer.DeleteContainerAsync();
            await OrdersContainer.DeleteContainerAsync();

        }

        public async Task DeleteDatabase()
        {
            await _database.DeleteAsync();
        }

    }
}
