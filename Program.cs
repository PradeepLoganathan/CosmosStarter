using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosStarter
{
    partial class Program
    {
        //private const string EndpointUri = "https://theta-cosmosdb-account.documents.azure.com:443/";
        //private const string PrimaryKey = "OZnTr2MTGOO4IGWXEBMs8nmAOKkhOoxDzgOc0lwvoXTDotvYYQz7LbnCMwfKj87iKfx7y1RUm7RRTPWlULTWQw==";

        public static async Task Main(string[] args)
        {
            await MainAsync();
        }

        private static async Task MainAsync()
        {
            try
            {
                //CosmosDriver cosmosDriver = new CosmosDriver();
                DataGenerator DataGen = new DataGenerator();
                //await cosmosDriver.CreateDatabaseAsync();
                //await cosmosDriver.CreateContainerAsync();

                var orders = DataGen.SeedOrderData(10);
               

                var customer = DataGen.SeedCustomerData(orders);

                await DataGen.SerializeCustomerData(customer);
                await DataGen.SerializeOrderData(orders);

                //await cosmosDriver.AddData(customers);
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
