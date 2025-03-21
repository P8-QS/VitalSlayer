using System;

namespace UI
{
    public class EffectStringGenerator
    {
        public static string Fog(int level)
        {
            if (level is < 1 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Must be level 1 or 2.");
            }
            return $"Fog level {level}";
        }

        public static string FogDescription(int level)
        {
            return level switch
            {
                1 => "You can only see the current room and visited rooms on the minimap.",
                2 => "You can only see the current room on the minimap.",
                _ => throw new ArgumentOutOfRangeException(nameof(level), "Must be level 1 or 2.")
            };
        }

        public static string Hallucination(int level)
        {
            if (level is < 1 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Must be level 1 or 2.");
            }
            return $"Hallucination level {level}";
        }

        public static string HallucinationDescription(int level)
        {
            return level switch
            {
                1 => "Each room will contain a phantom enemy.",
                2 => "Each room will contain two phantom enemies.",
                _ => throw new ArgumentOutOfRangeException(nameof(level), "Must be level 1 or 2.")
            };
        }

        public static string AttackSpeed()
        {
            return "Increased attack speed";
        }

        public static string AttackSpeedDescription()
        {
            return "Your attack speed is increased by 20%.";
        }

        public static string Map(int size)
        {
            if (size is < 1 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Must be 1, 2, or 3.");
            }
            return $"Map size {size}";
        }

        public static string MapDescription(int size)
        {
            return size switch
            {
                1 => "The map will have three rooms.",
                2 => "The map will have six rooms.",
                3 => "The map will have nine rooms.",
                _ => throw new ArgumentOutOfRangeException(nameof(size), "Must be 1, 2, or 3.")
            };
        }
        
        public static string Scouting()
        {
            return "scouting ability";
        }
        
        public static string ScoutingDescription()
        {
            return "You can see adjacent rooms on the minimap. ";
        }
    }
}
