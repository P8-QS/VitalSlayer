using UnityEngine;

// Base combat stats for all fighting entities
[CreateAssetMenu(fileName = "NewFighterStats", menuName = "Combat Stats/Base Fighter Stats")]
public class BaseFighterStats : ScriptableObject
{
    [Header("Basic Stats")]
    public int baseHealth = 100;
    public float baseSpeed = 1.0f;
    public float healthScalingFactor = 1.2f;
    public float pushRecoverySpeed = 0.2f;
    public float immuneTime = 0.5f;

    public virtual int CalculateMaxHealth(int level)
    {
        return baseHealth + (int)(Mathf.Pow(level, healthScalingFactor));
    }
}