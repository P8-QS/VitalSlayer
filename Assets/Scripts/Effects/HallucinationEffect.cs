using System;
using UnityEngine;

namespace Effects {
    public class HallucinationEffect : IEffect
    {
        private int _level;
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
            EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();

            if (spawner == null)
            {
                Debug.LogError("EnemySpawner not found in the scene.");
                return;
            }

            int count = Level == 1 ? 1 : 2;

            for (int i = 0; i < count; i++)
            {
                Vector3 basePosition = GameManager.instance.player.transform.position;

                // Make sure spawn is not too close to the player
                Vector2 offset;
                do
                {
                    offset = UnityEngine.Random.insideUnitCircle * 3f;
                } while (offset.magnitude < 1.5f); // Set min spawn distance

                Vector3 spawnPosition = basePosition + new Vector3(offset.x, offset.y, 0);

                spawner.SpawnPhantomEnemy(spawnPosition);
            }
        }
        
    }
}