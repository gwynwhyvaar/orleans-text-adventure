using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Orleans;
using Orleans.Hosting;
namespace TextAdventure.Server.Console
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mapFileName = Path.Combine(path, "TextAdventureMap.json");
            switch (args.Length)
            {
                default:
                    System.Console.WriteLine("Invalid Command Line Arguments.");
                    return -1;
                case 0:
                    break;
                case 1:
                    mapFileName = args[0];
                    break;
            }
            if (File.Exists(mapFileName) is false)
            {
                System.Console.WriteLine($"File not found: {mapFileName}");
                return -2;
            }
            // configure the host
            using var host = Host.CreateDefaultBuilder().UseOrleans(siloHostBuilder =>
            {
                siloHostBuilder.UseLocalhostClustering();
            }).Build();
            // start the host
            await host.StartAsync();

            System.Console.WriteLine($"Map file name is '{mapFileName}'.");
            System.Console.WriteLine($"Setting up Text Adventure, please wait ...");

            // intialize the game world
            var client = host.Services.GetRequiredService<IGrainFactory>();
            var textAdventureGameServer = new TextAdventureServer(client);

            await textAdventureGameServer.ConfigureAsync(mapFileName);

            System.Console.WriteLine($"Setup completeed.");
            System.Console.WriteLine($"Now you can launch the client.");
            // exit when any key is pressed.
            System.Console.WriteLine($"Press any key to exit.");
            System.Console.ReadLine();

            await host.StopAsync();
            return 0;
        }
    }
}
