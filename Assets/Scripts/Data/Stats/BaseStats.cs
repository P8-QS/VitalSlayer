using UnityEngine;

[CreateAssetMenu(fileName = "NewFighterStats", menuName = "Combat Stats/Base Fighter Stats")]
public class BaseFighterStats : ScriptableObject
{
    [Header("Basic Stats")]
    public int baseHealth = 10;
    public float baseSpeed = 1.0f;
    public float healthScalingFactor = 2f;
    public float pushRecoverySpeed = 0.2f;
    public float immuneTime = 0.5f;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip hitSound;

    public virtual int CalculateMaxHealth(int level)
    {
        return baseHealth + (int)(Mathf.Pow(level - 1, healthScalingFactor));
    }

}