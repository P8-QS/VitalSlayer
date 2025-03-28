using UnityEngine;

namespace Effects {
    public class ScoutingEffect : IEffect
    {
        private int _level;
        private Sprite _icon;
        public string Name => "Scouting";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public ScoutingEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=green>scouting ability</color>";
        }
        public string Description()
        {
            return "You can see adjacent rooms on the minimap.";
        }

        public void Apply()
        {
            throw new System.NotImplementedException();
        }
    }
}