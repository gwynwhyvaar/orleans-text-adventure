using System.Threading.Tasks;

using TextAdventure.Entities;
using TextAdventure.Grain.Interfaces;

namespace TextAdventure.Server.Interfaces
{
    public interface ITextAdventureInterface
    {
        Task<IRoomGrain> MakeRoomAsync(RoomInfo data);
        Task MakeThingAsync(ThingInfo thing);
        Task MakeMonsterAsync(MonsterInfo data, IRoomGrain room);
        Task ConfigureAsync(string fileName);
    }
}
