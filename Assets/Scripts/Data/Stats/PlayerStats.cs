using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Combat Stats/Player Stats")]
public class PlayerStats : BaseFighterStats
{
    [Header("Player-Specific Stats")]
    public float attackCooldown = 2.0f;
    public int baseMinDamage = 1;
    public int baseMaxDamage = 5;
    public float damageScalingFactor = 1.15f;
    public float pushForce = 5.0f;
    public float critChance = 0.1f;
    public float critMultiplier = 2.0f;

    [Header("Attack Audio")]
    public AudioClip attackSound;
    public int CalculateMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMinDamage, level, damageScalingFactor);
    }

    public int CalculateMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(baseMaxDamage, level, damageScalingFactor);
    }
}