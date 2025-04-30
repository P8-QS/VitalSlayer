using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewStandardEnemy", menuName = "Enemy Stats/Standard Enemy")]
public class BaseEnemyStats : BaseFighterStats
{
    [Header("Basic Info")]
    public string enemyName;

    [Header("Base Stats")]
    public int baseMinDamage = 1;
    public int baseMaxDamage = 3;
    public float minDamageScalingFactor = 1.95f;
    public float maxDamageScalingFactor = 1.96f;

    [Header("Combat Settings")]
    public float pushForce = 2.0f;
    public float critChance = 0.1f;
    public float critMultiplier = 1.5f;

    [Header("AI Settings")]
    public float chaseLength = 5.0f;
    public float triggerLength = 1.0f;

    [Header("Drop Settings")]
    public int baseXpReward = 1;

    [Tooltip("Maximum bonus multiplier for fighting enemies above your level (1.5 = 50% more XP)")]
    [Range(1.0f, 3.0f)]
    public float aboveLevelXpMultiplier = 1.5f;

    [Tooltip("Minimum multiplier for fighting enemies below your level (0.5 = 50% less XP)")]
    [Range(0.1f, 1.0f)]
    public float belowLevelXpMinimum = 0.5f;

    [Tooltip("How quickly XP scales with level difference")]
    [Range(0.05f, 0.3f)]
    public float levelDifferenceImpact = 0.15f;


    public virtual int GetScaledHealth(int level)
    {
        if (level <= 0)
        {
            return baseHealth;
        }

        return baseHealth + (int)Mathf.Pow(level, healthScalingFactor);
    }

    public virtual int GetScaledMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMinDamage, level, minDamageScalingFactor);
    }

    public virtual int GetScaledMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMaxDamage, level, maxDamageScalingFactor);
    }

    public virtual int GetScaledXpReward(int level)
    {
        int playerLevel = ExperienceManager.Instance.Level;

        float baseXp = baseXpReward;

        int levelDifference = level - playerLevel;

        float xpMultiplier;

        if (levelDifference > 0)
        {
            xpMultiplier = Mathf.Min(
                1.0f + (levelDifference * levelDifferenceImpact),
                aboveLevelXpMultiplier
            );
        }
        else if (levelDifference < 0)
        {
            xpMultiplier = Mathf.Max(
                1.0f + (levelDifference * levelDifferenceImpact),
                belowLevelXpMinimum
            );
        }
        else
        {
            xpMultiplier = 1.0f;
        }

        int finalXp = Mathf.RoundToInt((baseXp * playerLevel) * xpMultiplier);
        return Mathf.Max(finalXp, 1);
    }

    public Damage CalculateDamageObject(int level, Vector3 origin)
    {
        int minDamage = GetScaledMinDamage(level);
        int maxDamage = GetScaledMaxDamage(level);
        int actualDamage = Random.Range(minDamage, maxDamage + 1);
        bool isCritical = Random.value < critChance;

        if (isCritical)
        {
            actualDamage = Mathf.RoundToInt(actualDamage * critMultiplier);
        }

        return new Damage
        {
            damageAmount = actualDamage,
            origin = origin,
            pushForce = pushForce,
            isCritical = isCritical,
            minPossibleDamage = minDamage,
            maxPossibleDamage = maxDamage,
            useCustomColor = false
        };
    }
}