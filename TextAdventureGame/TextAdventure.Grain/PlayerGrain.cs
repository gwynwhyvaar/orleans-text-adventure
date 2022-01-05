using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orleans;

using TextAdventure.Entities;
using TextAdventure.Grain.Interfaces;
namespace TextAdventure.Grain
{
    public class PlayerGrain : Orleans.Grain, IPlayerGrain
    {
        private IRoomGrain _roomGrain;
        private List<ThingInfo> _things;
        private bool _killed;
        private PlayerInfo _playerInfo;
        public PlayerGrain()
        {
            this._things = new List<ThingInfo>();
            this._killed = false;
        }
        public override Task OnActivateAsync()
        {
            this._playerInfo = new PlayerInfo
            {
                Key = this.GetPrimaryKey(),
                Name = "lost soul"
            };
            return base.OnActivateAsync();
        }
        public async Task DieAsync()
        {
            // drop everything
            var tasks = new List<Task<string>>();
            this._things.ForEach(_ => tasks.Add(DropAsync(_)));
            await Task.WhenAll(tasks);
            // exit the game
            if (this._roomGrain != null)
            {
                await this._roomGrain.ExitAsync(this._playerInfo);
                this._roomGrain = null;
                this._killed = true;
            }
        }
        public Task<string> NameAsync()
        {
            return Task.FromResult(this._playerInfo.Name);
        }
        public async Task<string> PlayAsync(string command)
        {
            ThingInfo thingInfo;
            command = command.RemoveStopWords();
            string[] words = command.Split(' '); // the first string is the verb
            string verb = words[0].ToLower();
            if (this._killed && verb != "end")
            {
                return await CheckAliveAsync();
            }
            switch (verb)
            {
                case "look":
                    return await this._roomGrain.DescriptionAsync(this._playerInfo);
                case "go":
                    if (words.Length == 1)
                    {
                        return "Go Where?";
                    }
                    return await GoAsync(words[1]);
                case "north":
                case "south":
                case "east":
                case "west":
                    return await GoAsync(verb);
                case "kill":
                    if (words.Length == 1)
                    {
                        return "Kill what?";
                    }
                    var target = command.Substring(verb.Length + 1);
                    return await KillAsync(target);
                case "drop":
                    // get a thing based on the item name
                    thingInfo = FindMyThing(words.Rest());
                    return await DropAsync(thingInfo);
                case "take":
                    thingInfo = await this._roomGrain.FindThingAsync(words.Rest());
                    return await TakeAsync(thingInfo);
                case "inv":
                case "inventory":
                    return $"You are carrying {string.Join(string.Empty, this._things.Select(_ => _.Name))}";
                case "end":
                    return string.Empty;
            }
            return "I don't understand";
        }
        private async Task<string> TakeAsync(ThingInfo thingInfo)
        {
            if (this._killed)
            {
                return await CheckAliveAsync();
            }
            if (thingInfo != null)
            {
                this._things.Add(thingInfo);
                await this._roomGrain.TakeAsync(thingInfo);
                return "Okay";
            }
            else
            {
                return "I don't understand..";
            }
        }
        private async Task<string> DropAsync(ThingInfo thingInfo)
        {
            if (this._killed)
            {
                return await CheckAliveAsync();
            }
            if (thingInfo != null)
            {
                this._things.Remove(thingInfo);
                await this._roomGrain.DropAsync(thingInfo);
                return "Okay";
            }
            else
            {
                return "I don't understand..";
            }
        }
        public Task<IRoomGrain> RoomGrainAsync()
        {
            return Task.FromResult(this._roomGrain);
        }
        public Task SetNameAsync(string name)
        {
            this._playerInfo.Name = name;
            return Task.CompletedTask;
        }
        public Task SetRoomGrainAsync(IRoomGrain roomGrain)
        {
            this._roomGrain = roomGrain;
            return roomGrain.EnterAsync(this._playerInfo);
        }
        private async Task<string> GoAsync(string direction)
        {
            var destination = await this._roomGrain.ExitToAsync(direction);
            var description = new StringBuilder();
            if (description != null)
            {
                await this._roomGrain.ExitAsync(this._playerInfo); // todo: use generics.
                await destination.EnterAsync(this._playerInfo);
                this._roomGrain = destination;
                var desc = await destination.DescriptionAsync(this._playerInfo);
                if (desc != null)
                {
                    description.Append(desc);
                }
            }
            else
            {
                description.Append("You cannot go in that direction.");
            }
            if (this._things.Count > 0)
            {
                description.AppendLine("You are holding the following item(s): ");
                this._things.ForEach(_ => description.AppendLine(_.Name));
            }
            return description.ToString();
        }
        private async Task<string> CheckAliveAsync()
        {
            if (this._killed is false)
            {
                return null;
            }
            // Go to room '-2', which is the place of no return.
            var room = GrainFactory.GetGrain<IRoomGrain>(-2);
            return await room.DescriptionAsync(this._playerInfo);
        }
        private async Task<string> KillAsync(string target)
        {
            if (this._things.Count is 0)
            {
                return MessageConstants.NO_WEAPONS_MESSAGE;
            }
            var player = await this._roomGrain.FindPlayerAsync(target);
            if (player != null)
            {
                var weapon = this._things.Where(_ => _.Category is "weapon").FirstOrDefault();
                if (weapon != null)
                {
                    await GrainFactory.GetGrain<IPlayerGrain>(player.Key).DieAsync();
                    return $"{target} is now dead.";
                }
                return MessageConstants.NO_WEAPONS_MESSAGE;
            }
            var monster = await this._roomGrain.FindMonsterAsync(target);
            if (monster != null)
            {
                var weapons = monster.KilledBy.Join(this._things, id => id, t => t.Id, (id, t) => t);
                if (weapons.Count() > 0)
                {
                    await GrainFactory.GetGrain<IMonsterGrain>(monster.Id).KillAsync(this._roomGrain);
                    return $"{target} is now dead.";
                }
                return MessageConstants.NO_WEAPONS_MESSAGE;
            }
            return $"I can't see {target} here. Are you sure?";
        }
        private ThingInfo FindMyThing(string name)
        {
            return this._things.Where(_ => _.Name == name).FirstOrDefault();
        }
    }
}
