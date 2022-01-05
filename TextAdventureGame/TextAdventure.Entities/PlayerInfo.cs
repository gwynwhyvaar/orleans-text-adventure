using Orleans.Concurrency;
using System;
namespace TextAdventure.Entities
{
    [Immutable]
    public class PlayerInfo
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
    }
}
