using System;
using System.Threading.Tasks;

using Orleans;

using TextAdventure.Entities;
using TextAdventure.Grain.Interfaces;
namespace TextAdventure.Grain
{
    public class MonsterGrain : Orleans.Grain, IMonsterGrain
    {
        MonsterInfo _monsterInfo;
        IRoomGrain _roomGrain;
        public MonsterGrain()
        {
            this._monsterInfo = new MonsterInfo();
        }
        public override Task OnActivateAsync()
        {
            this._monsterInfo.Id = this.GetPrimaryKeyLong();
            RegisterTimer((_) => MoveAsync(), null, TimeSpan.FromSeconds(150), TimeSpan.FromMinutes(150));
            return base.OnActivateAsync();
        }

        private async Task MoveAsync()
        {
            var directions = new string[] { "north", "south", "west", "east" };
            var rand = new Random().Next(0, 4);
            var nextRoom = await this._roomGrain.ExitToAsync(directions[rand]);
            if (nextRoom is null)
            {
                return;
            }
            await this._roomGrain.ExitAsync(this._monsterInfo);
            await nextRoom.EnterAsync(this._monsterInfo);

            this._roomGrain = nextRoom;
        }

        public Task<string> KillAsync(IRoomGrain roomGrain)
        {
            if (this._roomGrain != null)
            {
                if (this._roomGrain.GetPrimaryKey() != roomGrain.GetPrimaryKey())
                {
                    var resultString = $"{this._monsterInfo.Name} snuck away. You were too slow!";
                    return Task.FromResult(resultString);
                }
                return this._roomGrain.ExitAsync(this._monsterInfo).ContinueWith(_ => $"{this._monsterInfo.Name} is dead");
            }
            return Task.FromResult($"{this._monsterInfo.Name} is already dead. You were too slow and someone else got to him!");
        }

        public Task<string> NameAsync()
        {
            return Task.FromResult(this._monsterInfo.Name);
        }

        public Task<IRoomGrain> RoomGrainAsync()
        {
            return Task.FromResult(this._roomGrain);
        }

        public Task SetInfoAsync(MonsterInfo monsterInfo)
        {
            this._monsterInfo = monsterInfo;
            return Task.CompletedTask;
        }

        public async Task SetRoomGrainAsync(IRoomGrain roomGrain)
        {
            if (this._roomGrain != null)
            {
                await this._roomGrain.ExitAsync(this._monsterInfo);
            }
            this._roomGrain = roomGrain;
            await this._roomGrain.EnterAsync(this._monsterInfo);
        }
    }
}
