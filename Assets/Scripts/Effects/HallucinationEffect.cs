using System;
using UnityEngine;

namespace Effects {
    public class HallucinationEffect : IEffect
    {
        private int _level;
        private int _numberOfPhantomEnemies;
        private Sprite _icon;
        public string Name => "Hallucination";

        public int Level { get => _level; set => _level = value; }
        public Sprite Icon { get => _icon; }

        public HallucinationEffect(Sprite icon, int level) {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=#EA2E1E>hallucination level {Level}</color>";
        }
        public string Description()
        {
            return Level switch
            {
                1 => "Each room will contain a phantom enemy.",
                2 => "Each room will contain two phantom enemies.",
                _ => throw new ArgumentOutOfRangeException(nameof(Level), "Must be level 1 or 2.")
            };
        }

        public void Apply()
        {
            // Apply the hallucination effect to the enemies in the room
            Enemy enemy = UnityEngine.Object.FindFirstObjectByType<Enemy>();
            
            if (_level == 1)
            {
                Debug.Log("Phantom enemy spawned");
                enemy.isPhantom = true;
                _numberOfPhantomEnemies = 1;
                enemy.hitpoint = 1;
                //spawing phantom enemy spawnPhamton(numberOfPhantomEnemies)
                enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // semi-transparent white
            }
            else if (_level == 2)
            {
                enemy.isPhantom = true;
                _numberOfPhantomEnemies = 2;
                enemy.hitpoint = 1;
                enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // semi-transparent white
            }
            
        }

       
    }
}
