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
            //enemy class has a method to spawn phantom enemies 
            //need to find a way to spawn phantom enemies
            Enemy enemy = UnityEngine.Object.FindFirstObjectByType<Enemy>();
            
            if (_level == 1)
            {
                Debug.Log("Phantom enemy spawned");
                _numberOfPhantomEnemies = 1;
                enemy.isPhantom = true;
                enemy.PhantomHitpoints = 1;
                //spawing phantom enemy where spawnPhamton(numberOfPhantomEnemies)/
                enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // semi-transparent white
            }
            else if (_level == 2)
            {
                _numberOfPhantomEnemies = 2;
                enemy.isPhantom = true;
                enemy.PhantomHitpoints = 1;
                enemy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // semi-transparent white
            }
            
        }

       
    }
}
