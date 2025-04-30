using UnityEngine;

/// <summary>
/// Runtime version of player stats that can be modified by perks
/// without affecting the original ScriptableObject
/// </summary>
public class RuntimePlayerStats
{
    // Reference to the original ScriptableObject
    private readonly PlayerStats _baseStats;

    // Runtime modifiable values
    public float AttackCooldown { get; set; }
    public int BaseMinDamage { get; set; }
    public int BaseMaxDamage { get; set; }
    public float MinDamageScalingFactor { get; set; }
    public float MaxDamageScalingFactor { get; set; }
    public float PushForce { get; set; }
    public float CritChance { get; set; }
    public float CritMultiplier { get; set; }
    public float BaseSpeed { get; set; }
    public int BaseHealth { get; set; }

    public AudioClip AttackSound => _baseStats.attackSound;
    public AudioClip DeathSound => _baseStats.deathSound;
    public AudioClip HitSound => _baseStats.hitSound;

    public float ImmunityTime => _baseStats.immuneTime;
    public float PushRecoverySpeed => _baseStats.pushRecoverySpeed;

    public RuntimePlayerStats(PlayerStats baseStats)
    {
        _baseStats = baseStats;
        Reset();
    }

    /// <summary>
    /// Reset all runtime values to their base values from the ScriptableObject
    /// </summary>
    public void Reset()
    {
        AttackCooldown = _baseStats.attackCooldown;
        BaseMinDamage = _baseStats.baseMinDamage;
        BaseMaxDamage = _baseStats.baseMaxDamage;
        MinDamageScalingFactor = _baseStats.minDamageScalingFactor;
        MaxDamageScalingFactor = _baseStats.maxDamageScalingFactor;
        PushForce = _baseStats.pushForce;
        CritChance = _baseStats.critChance;
        CritMultiplier = _baseStats.critMultiplier;
        BaseSpeed = _baseStats.baseSpeed;
        BaseHealth = _baseStats.baseHealth;
    }

    public int CalculateMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(BaseMinDamage, level, MinDamageScalingFactor);
    }

    public int CalculateMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(BaseMaxDamage, level, MaxDamageScalingFactor);
    }

    public int CalculateMaxHealth(int level)
    {
        return _baseStats.CalculateMaxHealth(level);
    }
}