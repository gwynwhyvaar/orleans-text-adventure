using Orleans.Concurrency;
using System.Collections.Generic;

namespace TextAdventure.Entities
{
    [Immutable]
    public class ThingInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public long FoundIn { get; set; }
        public List<string> Commands { get; set; }
    }
}
