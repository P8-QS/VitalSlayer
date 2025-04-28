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

    [Header("Toxic Puddle Settings")]
    public float puddleDuration = 5.0f;
    public float puddleSlowFactor = 0.5f;

    public int GetScaledAcidMinDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(acidMinDamage, level, damageScalingFactor);
    }

    public int GetScaledAcidMaxDamage(int level)
    {
        return GameHelpers.CalculateDamageStat(acidMaxDamage, level, damageScalingFactor);
    }
}