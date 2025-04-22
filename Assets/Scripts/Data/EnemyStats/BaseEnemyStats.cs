using UnityEngine;

[CreateAssetMenu(fileName = "NewStandardEnemy", menuName = "Enemy Stats/Standard Enemy")]
public class BaseEnemyStats : BaseFighterStats
{
    [Header("Basic Info")]
    public string enemyName;

    [Header("Base Stats")]
    public int baseMinDamage = 1;
    public int baseMaxDamage = 3;
    public float damageScalingFactor = 1.15f;

    [Header("Combat Settings")]
    public float pushForce = 2.0f;
    public float critChance = 0.1f;
    public float critMultiplier = 1.5f;

    [Header("AI Settings")]
    public float chaseLength = 5.0f;
    public float triggerLength = 1.0f;

    [Header("Drop Settings")]
    public int xpReward = 10;

    // Calculate scaled stats based on level
    public virtual int GetScaledHealth(int level)
    {
        return GameHelpers.CalculateDamageStat(baseHealth, level, healthScalingFactor);
    }

    public virtual int GetScaledMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMinDamage, level, damageScalingFactor);
    }

    public virtual int GetScaledMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMaxDamage, level, damageScalingFactor);
    }

    public virtual int GetScaledXpReward(int level)
    {
        return xpReward * (level * level);
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