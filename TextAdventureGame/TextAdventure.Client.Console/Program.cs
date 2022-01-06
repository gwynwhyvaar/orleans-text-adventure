using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Orleans;

using Spectre.Console;

using TextAdventure.Grain.Interfaces;

namespace TextAdventure.Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fontName = Path.Combine(path, "Fonts\\isometric1.flf");
            var font = FigletFont.Load(fontName);

            using var client = new ClientBuilder().UseLocalhostClustering().Build();
            await client.Connect();
            AnsiConsole.Write(new Rule("[green]Adventure Game Client[/]"));
            AnsiConsole.Write(new FigletText(font, "Adventure").Centered().Color(Color.Aqua));
            System.Console.WriteLine();
            AnsiConsole.MarkupLine("[bold grey]Welcome! by what name shall we call you?[/]");
            AnsiConsole.MarkupLine("[bold green]name?[/]");
            string name = System.Console.ReadLine();

            var player = client.GetGrain<IPlayerGrain>(Guid.NewGuid());
            await player.SetNameAsync(name);

            var room1 = client.GetGrain<IRoomGrain>(0);
            await player.SetRoomGrainAsync(room1);

            var description = await player.PlayAsync("look");
            AnsiConsole.MarkupLine($"[bold fuchsia]{DateTime.Now}[/] [bold blue]{description}[/]");
            System.Console.WriteLine();
            string result = "Start";
            try
            {
                while (result != string.Empty)
                {
                    string command = System.Console.ReadLine();
                    result = await player.PlayAsync(command);
                    AnsiConsole.MarkupLine($"[bold fuchsia]{DateTime.Now}[/] [bold yellow]{result}[/]");
                }
            }
            finally
            {
                await player.DieAsync();
                AnsiConsole.MarkupLine($"[bold fuchsia]{DateTime.Now}[/] [bold red]Game over![/]");
                await client.Close();
            }
        }
    }
}
