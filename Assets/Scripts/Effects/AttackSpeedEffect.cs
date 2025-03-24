using System;
using UnityEngine;

namespace Effects {
    public class AttackSpeedEffect : IEffect
    {
        private int _level;
        private Sprite _icon;
        public string Name => "Attack Speed";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public AttackSpeedEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=green>Increased attack speed</color>";
        }
        public string Description()
        {
            return "Your attack speed is increased by 20%.";
        }

    }
}