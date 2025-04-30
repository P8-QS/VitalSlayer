using UnityEngine;

namespace Effects
{
    public class CombatInfoEffect : IEffect
    {
        string IEffect.Name => "Combat Information";

        public int Level { get; set; }

        public Sprite Icon { get; }

        public CombatInfoEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }

        public string Text()
        {
            if (Level == 1)
            {
                return $"<color=#3AE75B>combat information</color>";
            }

            return $"<color=#EA2E1E>no combat information</color>";
        }

        public string Description()
        {
            if (Level == 1)
            {
                return "Enemies health bar will be visible.";
            }

            return "Enemies health bar will not be visible.";
        }

        public void Apply()
        {
        }
    }
}