using Microsoft.Azure.Cosmos;

namespace CosmosStarter
{
    public static class CosmosDbConnection
    {
        private const string EndpointUri = "https://localhost:8081";
        private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static CosmosClient _instance = null;
        private static readonly CosmosClientOptions CosmosClientOptions = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Direct,
            ApplicationName = "CosmosStarter",
            PortReuseMode = PortReuseMode.PrivatePortPool
        };


        public static CosmosClient Instance => _instance ??= new CosmosClient(EndpointUri, PrimaryKey, CosmosClientOptions);
    }
}
