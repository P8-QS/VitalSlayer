using System;
using UnityEngine;

namespace Effects {
    public class FogEffect : IEffect
    {
        private int _level;
        private Sprite _icon;
        public string Name => "Fog";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public FogEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=red>fog level {Level}</color>";
        }
        public string Description()
        {
            return Level switch
            {
                1 => "You can only see the current room and visited rooms on the minimap.",
                2 => "You can only see the current room on the minimap.",
                _ => throw new ArgumentOutOfRangeException(nameof(Level), "Must be level 1 or 2.")
            };
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}