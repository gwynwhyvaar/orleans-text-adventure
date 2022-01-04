using System.Collections.Generic;
namespace TextAdventure.Entities
{
    public class MapInfo
    {
        public string Name { get; set; }
        public List<RoomInfo> Rooms { get; set; }
        public List<CategoryInfo> Categories { get; set; }
        public List<ThingInfo> Things { get; set; }
        public List<MonsterInfo> Monsters { get; set; }
    }
}
