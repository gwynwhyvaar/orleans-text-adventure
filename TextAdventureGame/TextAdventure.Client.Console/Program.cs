using System;
using System.Threading.Tasks;

using Orleans;

using TextAdventure.Grain.Interfaces;

namespace TextAdventure.Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new ClientBuilder().UseLocalhostClustering().Build();
            await client.Connect();
            System.Console.WriteLine(@"
  ___      _                 _                  
 / _ \    | |               | |                 
/ /_\ \ __| |_   _____ _ __ | |_ _   _ _ __ ___ 
|  _  |/ _` \ \ / / _ \ '_ \| __| | | | '__/ _ \
| | | | (_| |\ V /  __/ | | | |_| |_| | | |  __/
\_| |_/\__,_| \_/ \___|_| |_|\__|\__,_|_|  \___|");
            System.Console.WriteLine();
            System.Console.WriteLine($"What's your name?");
            string name = System.Console.ReadLine();

            var player = client.GetGrain<IPlayerGrain>(Guid.NewGuid());
            await player.SetNameAsync(name);

            var room1 = client.GetGrain<IRoomGrain>(0);
            await player.SetRoomGrainAsync(room1);

            System.Console.WriteLine(await player.PlayAsync("look"));
            string result = "Start";
            try
            {
                while (result != string.Empty)
                {
                    string command = System.Console.ReadLine();
                    result = await player.PlayAsync(command);
                    System.Console.WriteLine(result);
                }
            }
            finally
            {
                await player.DieAsync();
                System.Console.WriteLine("Game over!");
                await client.Close();
            }
        }
    }
}
