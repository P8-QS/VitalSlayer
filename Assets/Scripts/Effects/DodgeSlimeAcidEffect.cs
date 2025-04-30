using UnityEngine;

namespace Effects{

    public class DodgeSlimeAcidEffect : IEffect
    {
        private int _level;
        public static float SlowFactorEffect { get; private set; } = 0.5f;  //  default value compared to slime puddle info.         
        public static bool puddleImmunityEffect {get; private set;} = false;  // default value
        private Sprite _icon;
        string IEffect.Name => "Dodge Slime Acid";
        
        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public DodgeSlimeAcidEffect(Sprite icon, int level)
        {
            _icon = icon;
            _level = level;
        }

        
        string IEffect.Text() => Level switch
        {
            0 => "<color=#EA2E1E>Level 0</color> - <color=#EA2E1E>25%</color> speed in slime puddles.",
            1 => "<color=#F5CC45>Level 1</color> - <color=#F5CC45>50%</color> speed in slime puddles.",
            2 => "<color=#3AE75B>Level 2</color> - <color=#3AE75B>Immune</color> to puddle slowdown and damage.",
            _ => throw new System.ArgumentOutOfRangeException(nameof(Level), "Must be level 0, 1 or 2.")
        };

        string IEffect.Description() =>
            "Reduces or removes slime puddle slowdown and damage based on level.";

        public void Apply()
        {
            switch (_level)
            {
            case 2:
                puddleImmunityEffect = true;
                break;
            case 1:
                SlowFactorEffect = 0.5f;
                puddleImmunityEffect = false;
                break;
            default:
                SlowFactorEffect = 0.25f;
                puddleImmunityEffect = false;
                break;
            }
        }
    }





}