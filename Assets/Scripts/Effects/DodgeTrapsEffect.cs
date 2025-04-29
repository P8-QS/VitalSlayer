using UnityEngine;

namespace Effects {
    public class DodgeTrapsEffect : IEffect
    {
        private int _level;
        private Sprite _icon;
        public string Name => "Dodge Traps";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public DodgeTrapsEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=#3AE75B>dodge spike traps ability</color>";
        }
        public string Description()
        {
            return "Spike traps will not activate, when you walk over them.";
        }

        public void Apply()
        {
            // Scuffed but handled by TrapManager
        }
    }
}