using System.Collections.Generic;

using Orleans.Concurrency;

namespace TextAdventure.Entities
{
    [Immutable]
    public class MapInfo
    {
        public string Name { get; set; }
        public List<RoomInfo> Rooms { get; set; }
        public List<CategoryInfo> Categories { get; set; }
        public List<ThingInfo> Things { get; set; }
        public List<MonsterInfo> Monsters { get; set; }
    }
}
