using Orleans.Concurrency;
using System.Collections.Generic;
namespace TextAdventure.Entities
{
    [Immutable]
    public class CategoryInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<string> Commands { get; set; }
    }
}