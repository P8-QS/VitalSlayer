
using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedEnemy", menuName = "Enemy Stats/Ranged Enemy")]
public class RangedEnemyStats : BaseEnemyStats
{
    [Header("Ranged Attack Settings")]
    public float attackCooldown = 2.0f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 3.0f;
    public float projectileLifetime = 2.0f;
    public int rangedMinDamage = 2;
    public int rangedMaxDamage = 4;
    public float retreatDistance = 0.5f;
    public float chaseDistance = 0.1f;

    public int GetScaledRangedMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(rangedMinDamage, level, damageScalingFactor);
    }

    public int GetScaledRangedMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(rangedMaxDamage, level, damageScalingFactor);
    }
}