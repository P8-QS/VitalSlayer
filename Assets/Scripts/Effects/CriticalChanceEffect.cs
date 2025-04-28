using System;
using UnityEngine;

namespace Effects
{
    public class CriticalChanceEffect : IEffect
    {
        public string Name => "Critical Chance";
        public int Level { get; set; }
        public Sprite Icon { get; }
     
        public CriticalChanceEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }
        
        public string Text()
        {
            return $"<color=#3AE75B>critical chance level {Level}</color>";
        }

        public string Description()
        {
            return Level switch
            {
                2 => "Increased critical chance by 20%!",
                1 => "Increased critical chance by 10%!",
                _ => throw new ArgumentOutOfRangeException(nameof(Level), "Must be level 1 or 2.")
            };
        }

        public void Apply()
        {
            throw new System.NotImplementedException();
        }
    }
}