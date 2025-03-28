using System;
using UnityEngine;

namespace Effects {
    public class AttackSpeedEffect : IEffect
    {
        private int _level;
        public float attackSpeedMultiplier = 0.8f;
        private Sprite _icon;
        string IEffect.Name => "Attack Speed";

        int IEffect.Level { get => _level; set => _level = value; }
        Sprite IEffect.Icon { get => _icon; }

        public AttackSpeedEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        string IEffect.Text()
        {
            return $"<color=green>Increased attack speed</color>";
        }
        string IEffect.Description()
        {
            return "Your attack speed is increased by 20%.";
        }
        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}