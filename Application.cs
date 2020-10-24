using CosmosStarter.Configuration;
using CosmosStarter.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CosmosStarter
{
    public class Application
    {
        private CosmosConfig cosmosConfig;
        private IServiceProvider serviceProvider;
       
        private void ConfigureApplication()
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            cosmosConfig = new CosmosConfig();
            Configuration.Bind("connectionSettings", cosmosConfig);

        }

        private void RegisterServices()
        {
            
            var services = new ServiceCollection();
            services.AddSingleton(cosmosConfig);
            services.AddSingleton<ICosmosDBContext, CosmosDBContext>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<ICosmosDriver, CosmosDriver>();
            serviceProvider = services.BuildServiceProvider();
        }

        private void DisposeServices()
        {
            if (serviceProvider == null)
            {
                return;
            }
            if (serviceProvider is IDisposable)
            {
                ((IDisposable)serviceProvider).Dispose();
            }

        }

        public async Task Run()
        {
            ConfigureApplication();
            RegisterServices();
            IServiceScope scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ICosmosDBContext>().Initialize();
            await scope.ServiceProvider.GetRequiredService<ICosmosDriver>().Drive();
            DisposeServices();

        }
    }
}
