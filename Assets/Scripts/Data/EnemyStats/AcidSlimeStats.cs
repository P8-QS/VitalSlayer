using UnityEngine;

[CreateAssetMenu(fileName = "NewAcidSlimeEnemy", menuName = "Enemy Stats/Acid Slime Enemy")]
public class AcidSlimeEnemyStats : BaseEnemyStats
{
    [Header("Acid Effect Settings")]
    public int acidMinDamage = 1;
    public int acidMaxDamage = 2;
    public float acidDuration = 3.0f;
    public float acidTickRate = 1.0f;

    public Color acidColor = Color.green;

    [Header("Level-Based Colors")]
    [Tooltip("Define colors for different level ranges")]
    public SlimeLevelColor[] levelColors = new SlimeLevelColor[]
    {
        new SlimeLevelColor(1, 3, Color.green),      // Light green for levels 1-3
        new SlimeLevelColor(4, 7, new Color(0f, 0.8f, 0f)), // Darker green for levels 4-7
        new SlimeLevelColor(8, 12, new Color(0.8f, 1f, 0f)), // Yellow-green for levels 8-12
        new SlimeLevelColor(13, int.MaxValue, new Color(1f, 0.5f, 0f)) // Orange for levels 13+
    };

    [Header("Toxic Puddle Settings")]
    public float puddleDuration = 5.0f;
    public float puddleSlowFactor = 0.5f;

    public int GetScaledAcidMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(acidMinDamage, level, minDamageScalingFactor);
    }

    public int GetScaledAcidMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(acidMaxDamage, level, maxDamageScalingFactor);
    }

    /// <summary>
    /// Gets the appropriate color for the enemy based on its level
    /// </summary>
    public Color GetColorForLevel(int level)
    {
        foreach (var levelColor in levelColors)
        {
            if (level >= levelColor.MinLevel && level <= levelColor.MaxLevel)
            {
                return levelColor.Color;
            }
        }

        // Return default color if no matching level range found
        return acidColor;
    }
}

[System.Serializable]
public class SlimeLevelColor
{
    [Tooltip("Minimum level for this color (inclusive)")]
    public int MinLevel = 1;

    [Tooltip("Maximum level for this color (inclusive)")]
    public int MaxLevel = 5;

    [Tooltip("Color to apply for enemies within this level range")]
    public Color Color = Color.green;

    public SlimeLevelColor(int minLevel, int maxLevel, Color color)
    {
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        Color = color;
    }
}