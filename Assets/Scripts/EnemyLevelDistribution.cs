using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLevelDistribution", menuName = "Combat/Enemy Level Distribution")]
public class EnemyLevelDistribution : ScriptableObject
{
    [System.Serializable]
    public class LevelChance
    {
        [Tooltip("Level difference from player (negative = below, positive = above)")]
        public int levelDifference;

        [Tooltip("Probability weight for this level")]
        [Range(0, 100)]
        public int weight = 10;

        [Tooltip("Description for this tier (for editor clarity)")]
        public string description;
    }

    [Tooltip("Chances for different enemy levels relative to player level")]
    public List<LevelChance> levelChances = new List<LevelChance>();

    [Tooltip("Minimum enemy level regardless of calculation")]
    public int minimumLevel = 1;

    [Tooltip("Boss enemies get this additional level bonus")]
    public int bossLevelBonus = 2;

    public void ValidateWeights()
    {
        if (levelChances.Count == 0)
        {
            Debug.LogWarning($"No level chances defined in {name}!");
        }
    }
}