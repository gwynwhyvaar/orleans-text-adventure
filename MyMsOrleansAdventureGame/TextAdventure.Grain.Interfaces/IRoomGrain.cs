using Orleans;
using System.Threading.Tasks;
using TextAdventure.Entities;
namespace TextAdventure.Grain.Interfaces
{
    public interface IRoomGrain: IGrainWithIntegerKey
    {
        Task<string> DescriptionAsync(PlayerInfo playerInfo);
        Task SetInfoAsync(RoomInfo roomInfo);
        Task<IRoomGrain> ExitToAsync(string direction);
        Task EnterAsync(PlayerInfo playerInfo);
        Task ExitAsync(PlayerInfo playerInfo);
        Task EnterAsync(MonsterInfo monsterInfo);
        Task ExitAsync(MonsterInfo monsterInfo);
        Task DropAsync(ThingInfo thingInfo);
        Task TakeAsync(ThingInfo thingInfo);
        Task<ThingInfo> FindThingAsync(string name);
        Task<PlayerInfo> FindPlayerAsync(string name);
        Task<MonsterInfo> FindMonsterAsync(string name);
    }
}