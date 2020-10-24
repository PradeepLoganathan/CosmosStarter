using System.Threading.Tasks;

namespace CosmosStarter
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Application app = new Application();
            await app.Run();
        }

    }
}
