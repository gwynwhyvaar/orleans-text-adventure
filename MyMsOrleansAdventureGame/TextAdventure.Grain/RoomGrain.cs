using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextAdventure.Entities;
using TextAdventure.Grain.Interfaces;
namespace TextAdventure.Grain
{
    public class RoomGrain : Orleans.Grain, IRoomGrain
    {
        private string _description;
        private List<PlayerInfo> _playerInfos;
        private List<MonsterInfo> _monsterInfos;
        private List<ThingInfo> _thingInfos;
        private Dictionary<string, IRoomGrain> _exits;
        public RoomGrain()
        {
            this._playerInfos = new List<PlayerInfo>();
            this._monsterInfos = new List<MonsterInfo>();
            this._thingInfos = new List<ThingInfo>();
            this._exits = new Dictionary<string, IRoomGrain>();
        }
        public Task<string> DescriptionAsync(PlayerInfo playerInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(this._description);
            if (this._thingInfos.Count > 0)
            {
                stringBuilder.AppendLine($"The following things are present: ");
                this._thingInfos.ForEach(_ => stringBuilder.Append(" ").AppendLine(_.Name));
            }
            var others = this._playerInfos.Where(_ => _.Key != playerInfo.Key).ToList();
            if (others.Count > 0 || this._monsterInfos.Count > 0)
            {
                stringBuilder.AppendLine("Beware! these guys are in the room with you: ");
                if (others.Count > 0)
                {
                    others.ForEach(_ => stringBuilder.Append(" ").AppendLine(_.Name));
                }
                if(this._monsterInfos.Count > 0)
                {
                    this._monsterInfos.ForEach(_ => stringBuilder.Append(" ").AppendLine(_.Name));
                }
            }
            return Task.FromResult(stringBuilder.ToString());
        }
        public Task DropAsync(ThingInfo thingInfo)
        {
            this._thingInfos.RemoveAll(_ => _.Id == thingInfo.Id);
            this._thingInfos.Add(thingInfo);
            return Task.CompletedTask;
        }
        public Task EnterAsync(PlayerInfo playerInfo)
        {
            this._playerInfos.RemoveAll(_ => _.Key == playerInfo.Key);
            this._playerInfos.Add(playerInfo);
            return Task.CompletedTask;
        }
        public Task EnterAsync(MonsterInfo monsterInfo)
        {
            this._monsterInfos.RemoveAll(_ => _.Id == monsterInfo.Id);
            this._monsterInfos.Add(monsterInfo);
            return Task.CompletedTask;
        }
        public Task ExitAsync(PlayerInfo playerInfo)
        {
            this._playerInfos.RemoveAll(_ => _.Key == playerInfo.Key);
            return Task.CompletedTask;
        }
        public Task ExitAsync(MonsterInfo monsterInfo)
        {
            this._monsterInfos.RemoveAll(_ => _.Id == monsterInfo.Id);
            return Task.CompletedTask;
        }
        public Task<IRoomGrain> ExitToAsync(string direction)
        {
            return Task.FromResult((this._exits.ContainsKey(direction)) ? this._exits[direction] : null);
        }
        public Task<MonsterInfo> FindMonsterAsync(string name)
        {
            name = name.ToLower();
            return Task.FromResult(this._monsterInfos.Where(_ => _.Name.ToLower().Contains(name)).FirstOrDefault());
        }
        public Task<PlayerInfo> FindPlayerAsync(string name)
        {
            name = name.ToLower();
            return Task.FromResult(this._playerInfos.Where(_ => _.Name.ToLower().Contains(name)).FirstOrDefault());
        }
        public Task<ThingInfo> FindThingAsync(string name)
        {
            return Task.FromResult(this._thingInfos.Where(_ => _.Name == name).FirstOrDefault());
        }
        public Task SetInfoAsync(RoomInfo roomInfo)
        {
            this._description = roomInfo.Description;
            var directions = roomInfo.Directions;
            foreach (var kv in directions)
            {
                this._exits[kv.Key] = GrainFactory.GetGrain<IRoomGrain>(kv.Value);
            }
            return Task.CompletedTask;
        }
        public Task TakeAsync(ThingInfo thingInfo)
        {
            this._thingInfos.RemoveAll(_ => _.Name == thingInfo.Name);
            return Task.CompletedTask;
        }
    }
}
