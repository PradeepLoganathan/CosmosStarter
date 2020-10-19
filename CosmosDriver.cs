using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CosmosStarter.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosStarter
{
    partial class Program
    {
        private class CosmosDriver
        {
            private readonly CosmosClient _cosmosClient;
            private Database database;
            private Container container;

            private const string EndpointUri = "<<ENDPOINTURL>>";
            private const string PrimaryKey = "<<YOURPRIMARYKEY>>";
            private string databaseId = "thetaDb";
            private string containerId = "CustomerContainer";
            public CosmosDriver()
            {
                _cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            }
            public async Task CreateDatabaseAsync()
            {
                try
                {
                    this.database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public async Task CreateContainerAsync()
            {
                this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/CompanyName");
            }
            
            public async Task AddData(List<Customer> customers)
            {
                foreach (var customer in customers)
                {
                    try
                    {
                        var customerResponse = await this.container.ReadItemAsync<Customer>(customer.CustomerId, new PartitionKey(customer.CompanyName));
                        Console.WriteLine("Item in database with id: {0} already exists\n", customer.CustomerId);
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        ItemResponse<Customer> customerResponse = await this.container.CreateItemAsync<Customer>(customer, new PartitionKey(customer.CompanyName));
                        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", customerResponse.Resource.CustomerId, customerResponse.RequestCharge);
                    }
                }
            }
        }
    }
}
