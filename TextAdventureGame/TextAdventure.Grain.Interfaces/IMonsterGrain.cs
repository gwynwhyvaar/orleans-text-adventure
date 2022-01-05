using Orleans;
using System.Threading.Tasks;
using TextAdventure.Entities;
namespace TextAdventure.Grain.Interfaces
{
    public interface IMonsterGrain : IGrainWithIntegerKey
    {
        Task<string> NameAsync();
        Task SetInfoAsync(MonsterInfo monsterInfo);
        Task SetRoomGrainAsync(IRoomGrain roomGrain);
        Task<IRoomGrain> RoomGrainAsync();
        Task<string> KillAsync(IRoomGrain roomGrain);
    }
 }
