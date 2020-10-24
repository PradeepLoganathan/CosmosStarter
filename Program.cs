using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosStarter
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await MainAsync();
        }

        private static async Task MainAsync()
        {
            try
            {
                CosmosDriver cosmosDriver = new CosmosDriver();
                DataGenerator dataGenerator = new DataGenerator();
                await cosmosDriver.CreateDatabaseAsync();
                await cosmosDriver.CreateContainersAsync();

                var orders = dataGenerator.SeedOrderData(10);
                var customer = dataGenerator.SeedCustomerData(orders);
                dataGenerator.AddOrdersToCustomer(orders, customer.CustomerId);

                //await dataGenerator.SerializeCustomerData(customer);
                //await dataGenerator.SerializeOrderData(orders);

                await cosmosDriver.AddCustomerStream(customer);

                await cosmosDriver.AddCustomer(customer);
                await cosmosDriver.AddOrders(orders, customer.CustomerId);

                var customerindb = await cosmosDriver.GetCustomer("CU7-36-8183");
                var ordersindb = await cosmosDriver.GetOrders("CU7-36-8183");
            }
            catch (CosmosException cosmosException)
            {
                Console.WriteLine(cosmosException);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
