using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon;
using UnityEngine;

namespace Effects
{
    public class LevelIndicatorEffect : IEffect
    {
        public string Name => "Enemy Level Indicator";
        public int Level { get; set; }

        public Sprite Icon { get; }

        public LevelIndicatorEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }

        public string Text()
        {
            return Level switch
            {
                1 => $"<color=#3AE75B>level indicators on enemies</color>",
                _ => $"<color=#EA2E1E>invisible level indicators on enemies</color>"
            };
        }

        public string Description()
        {
            return Level switch
            {
                1 => "Enemies will have a level indicator above them.",
                0 => "Enemies will not have a level indicator above them.",
                _ => "Must be level 0 or 1."
            };
        }

        public void Apply()
        {
        }
    }
}