using UnityEngine;

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

    // Multipliers
    public float DamageMultiplier { get; set; } = 1.0f;
    public float HealthMultiplier { get; set; } = 1.0f;

    // Audio references and other properties
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

        DamageMultiplier = 1.0f;
        HealthMultiplier = 1.0f;
    }

    public int CalculateMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(BaseMinDamage, level, MinDamageScalingFactor, DamageMultiplier);
    }

    public int CalculateMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(BaseMaxDamage, level, MaxDamageScalingFactor, DamageMultiplier);
    }

    public int CalculateMaxHealth(int level)
    {
        int baseCalculation = BaseHealth + (int)Mathf.Pow(level - 1, _baseStats.healthScalingFactor);
        return Mathf.RoundToInt(baseCalculation * HealthMultiplier);
    }
}