using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon;
using UnityEngine;

namespace Effects
{
    public class NoDoorCloseEffect : IEffect
    {
        public string Name => "Doors Always Open";
        public int Level { get; set; }

        public Sprite Icon { get; }

        public NoDoorCloseEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }

        public string Text()
        {
            return Level switch
            {
                1 => $"<color=#3AE75B>always opened doors</color>",
                _ => $"<color=#EA2E1E>closed doors</color>"
            };
        }

        public string Description()
        {
            return Level switch
            {
                1 => "Doors will never be closed.",
                0 => "Doors will be closed, and you need to kill all the enemies within the room for doors to open.",
                _ => "Must be level 0 or 1."
            };
        }

        public void Apply()
        {
            if (Level != 1) return;
            if (DungeonGenerator.Instance == null) return;

            var rooms = DungeonGenerator.Instance.PlacedRooms
                .Select(room => room.RoomScript);

            foreach (var room in rooms)
                room.DoorsAlwaysOpen = true;
        }
    }
}