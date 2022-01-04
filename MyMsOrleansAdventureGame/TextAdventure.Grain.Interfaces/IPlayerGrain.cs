using Orleans;
using System.Threading.Tasks;
namespace TextAdventure.Grain.Interfaces
{
    public interface IPlayerGrain: IGrainWithGuidKey
    {
        Task<string> NameAsync();
        Task SetNameAsync(string name);
        Task SetRoomGrainAsync(IRoomGrain roomGrain);
        Task<IRoomGrain> RoomGrainAsync();
        // death is a task waiting to happen ... sigh :/
        Task DieAsync();
        Task<string> PlayAsync(string command);
    }
}
