using UnityEngine;

namespace Effects
{
    public class AcidSlimePuddleEffect : IEffect
    {
        public static float SlowFactor { get; set; } = 0.5f;
        public static bool PuddleImmunity { get; private set; }
        public string Name => "Acid Slime Puddle";

        public Sprite Icon { get; }
        public int Level { get; set; }


        public AcidSlimePuddleEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }


        public string Text()
        {
            return Level switch
            {
                3 => "<color=#3AE75B>immunity to slime puddles.</color>",
                2 => "<color=#F5CC45>slowdown in slime puddles.</color>",
                _ => "<color=#EA2E1E>slowdown in slime puddles.</color>",
            };
        }

        public string Description()
        {
            return Level switch
            {
                3 => "You are immune to slime puddles.",
                2 => "You are slowed down in slime puddles and takes damage.",
                _ => "You are severely slowed down in slime puddles and takes damage."
            };
        }

        public void Apply()
        {
            switch (Level)
            {
                case 3:
                    PuddleImmunity = true;
                    break;
                case 2:
                    SlowFactor = 0.25f;
                    PuddleImmunity = false;
                    break;
                default:
                    SlowFactor = 0.5f;
                    PuddleImmunity = false;
                    break;
            }
        }
    }
}