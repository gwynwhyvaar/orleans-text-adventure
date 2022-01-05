using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
            if (File.Exists(mapFileName))
            {
                System.Console.WriteLine($"File not found: {mapFileName}");
                return -2;
            }
        }
    }
}
