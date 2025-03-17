using UnityEngine;

namespace Effects {
    public class SleepEffect : IEffect
    {
        private int _level;
        public string Name => "Map";
        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Text()
        {
            return $"map size {Level}";
        }
        public string Description()
        {
            throw new System.NotImplementedException();
        }

    }
}