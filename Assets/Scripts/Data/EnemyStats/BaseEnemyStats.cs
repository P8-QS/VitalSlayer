using UnityEngine;

[CreateAssetMenu(fileName = "NewStandardEnemy", menuName = "Enemy Stats/Standard Enemy")]
public class BaseEnemyStats : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public Sprite enemySprite;
    public RuntimeAnimatorController animatorController;
    public GameObject prefab;
    public bool isBoss = false;

    [Header("Base Stats")]
    public int baseHealth = 10;
    public int baseMinDamage = 1;
    public int baseMaxDamage = 3;
    public float baseSpeed = 1.0f;
    public float pushRecoverySpeed = 0.2f;
    public float damageScalingFactor = 1.15f;
    public float healthScalingFactor = 1.2f;

    [Header("Combat Settings")]
    public float pushForce = 2.0f;
    public float critChance = 0.1f;
    public float critMultiplier = 1.5f;

    [Header("AI Settings")]
    public float chaseLength = 5.0f;
    public float triggerLength = 1.0f;

    [Header("Drop Settings")]
    public int xpReward = 10;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip hitSound;

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
}