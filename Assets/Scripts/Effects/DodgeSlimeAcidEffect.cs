using UnityEngine;

namespace Effects{

    public class DodgeSlimeAcidEffect : IEffect
    {
        private int _level;
        public static float SlowFactorEffect { get; private set; } = 0.5f;  //  default value         
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

        
        string IEffect.Text()
        {
            return Level switch
            {
                1 => "<color=#EA2E1E>Level 1</color> - <color=#EA2E1E>25%</color>Extreme slime puddle slowdown.",
                2 => "<color=#F5CC45>Level 2</color> - <color=#F5CC45>50%</color> Reduced slime puddle slowdown.",
                3 => "<color=#3AE75B>Level 3</color> - <color=#3AE75B>No</color> No puddle slowdown or damage over time.",
                _ => throw new System.ArgumentOutOfRangeException(nameof(Level), "Must be level 1, 2 or 3.")
            };
        }
        
        string IEffect.Description()
        {
            return "Depending on level, you will be slowed down by slime puddles more or less, you can also become immune to their damage.";
        }
        
        public void Apply()
        {
            if(_level == 3){
                //Good
                //SlowFactorEffect = 1f;
                puddleImmunityEffect = true;


            }
            else if(_level == 1){
                //Bad
                SlowFactorEffect = 0.25f;
                puddleImmunityEffect = false;

            }
            else{
                //Neutral (E.g. Lvl 2)
                SlowFactorEffect = 0.5f;
                puddleImmunityEffect = false;



            }
        }
    }





}