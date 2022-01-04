﻿using Orleans.Concurrency;
using System.Collections.Generic;
namespace TextAdventure.Entities
{
    [Immutable]
    public class RoomInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, long> Directions { get; set; }
    }
}