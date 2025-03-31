using System;
using UnityEngine;

namespace Effects {
    public class MapEffect : IEffect
    {
        private int _level;
        private Sprite _icon;
        public string Name => "Map";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public MapEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"map size {Level}";
        }
        public string Description()
        {
            return Level switch
            {
                1 => "The map will have three rooms.",
                2 => "The map will have six rooms.",
                3 => "The map will have nine rooms.",
                _ => throw new ArgumentOutOfRangeException(nameof(Level), "Must be 1, 2, or 3.")
            };
        }

        public void Apply()
        {
            Debug.LogWarning("Apply map effect not implemented");
        }
    }
}