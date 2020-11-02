using CosmosStarter.Configuration;
using CosmosStarter.Interfaces;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Azure.Cosmos;

namespace CosmosStarter
{
    public class CosmosDbContext : ICosmosDbContext
    {
        private readonly CosmosConfig _cosmosConfig;
        private CosmosClient _cosmosClient;
        private CosmosDatabase _database;

        private const string DatabaseId = "CustomersDb";
        private const string CustomerContainerId = "CustomerContainer";
        private const string OrderContainerId = "OrderContainer";

        public CosmosContainer CustomerContainer { get; private set; }
        public CosmosContainer OrdersContainer { get; private set; }

        public CosmosDbContext(CosmosConfig cosmosConfig)
        {
            this._cosmosConfig = cosmosConfig;
        }

        public async Task Initialize()
        {
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Direct,
                ApplicationName = "CosmosStarter",
            };

            _cosmosClient = new CosmosClient(_cosmosConfig.Endpoint, _cosmosConfig.Key, cosmosClientOptions);

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
