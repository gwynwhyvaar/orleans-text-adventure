using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Orleans;

using TextAdventure.Entities;
using TextAdventure.Grain.Interfaces;
using TextAdventure.Server.Interfaces;

namespace TextAdventure.Server
{
    public class TextAdventureServer : ITextAdventureInterface
    {
        private readonly IGrainFactory _grainFactory;
        public TextAdventureServer(IGrainFactory grainFactory)
        {
            this._grainFactory = grainFactory;
        }
        public async Task ConfigureAsync(string fileName)
        {
            var rand = new Random();
            var jsonData = await File.ReadAllTextAsync(fileName);
            var data = await Task.Run(() => JsonSerializer.Deserialize<MapInfo>(jsonData));
            // init the game world with game data
            var rooms = new List<IRoomGrain>();
            var gameRooms = data.Rooms;
            var gameThings = data.Things;
            var gameMonsters = data.Monsters;
            foreach (var room in gameRooms)
            {
                var roomGrain = await MakeRoomAsync(room);
                if (room.Id >= 0)
                {
                    rooms.Add(roomGrain);
                }
            }
            foreach (var thing in gameThings)
            {
                await MakeThingAsync(thing);
            }
            foreach (var monster in gameMonsters)
            {
                await MakeMonsterAsync(monster, rooms[rand.Next(0, rooms.Count)]);
            }
        }

        public async Task MakeMonsterAsync(MonsterInfo data, IRoomGrain room)
        {
            var monsterGrain = this._grainFactory.GetGrain<IMonsterGrain>(data.Id);
            await monsterGrain.SetInfoAsync(data);
            await monsterGrain.SetRoomGrainAsync(room);
        }

        public async Task<IRoomGrain> MakeRoomAsync(RoomInfo data)
        {
            var roomGrain = this._grainFactory.GetGrain<IRoomGrain>(data.Id);
            await roomGrain.SetInfoAsync(data);
            return roomGrain;
        }

        public async Task MakeThingAsync(ThingInfo thing)
        {
            var roomGrain = this._grainFactory.GetGrain<IRoomGrain>(thing.FoundIn);
            await roomGrain.DropAsync(thing);
        }
    }
}
