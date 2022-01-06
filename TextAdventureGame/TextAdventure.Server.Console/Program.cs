using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Orleans;
using Orleans.Hosting;

using Spectre.Console;

namespace TextAdventure.Server.Console
{
    class Program
    {
        const string SERVER_BANNER = @"Adventure Server";
        static async Task<int> Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mapFileName = Path.Combine(path, "TextAdventureMap.json");
            var fontName = Path.Combine(path, "Fonts\\isometric1.flf");
            var font = FigletFont.Load(fontName);
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

            var fileName = Path.GetFileName(mapFileName);

            AnsiConsole.Write(new FigletText(font, SERVER_BANNER).LeftAligned().Color(Color.Aqua));
            AnsiConsole.MarkupLine($"Map file name is [bold red]'{fileName}'[/].");

            await AnsiConsole.Status().StartAsync("Setting up Text Adventure Server ", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.Status = "please wait ...";
                // intialize the game world
                var client = host.Services.GetRequiredService<IGrainFactory>();
                var textAdventureGameServer = new TextAdventureServer(client);

                await textAdventureGameServer.ConfigureAsync(mapFileName);

                AnsiConsole.MarkupLine($"Setup [bold blue]completed.[/]");
                AnsiConsole.MarkupLine($"Now you can [bold red]launch[/] the [bold green]client.[/]");
                // exit when any key is pressed.
                AnsiConsole.MarkupLine($"Press any [bold red]key to exit.[/]");
                ctx.Status = "completed.";
            });
            
            System.Console.ReadLine();

            await host.StopAsync();
            return 0;
        }
    }
}
