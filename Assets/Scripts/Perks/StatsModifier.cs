using UnityEngine;
using System.Collections.Generic;

public class StatsModifier : MonoBehaviour
{
    public static StatsModifier Instance { get; private set; }

    private Dictionary<StatType, float> statMultipliers = new Dictionary<StatType, float>();

    public enum StatType
    {
        Health,
        DamageMin,
        DamageMax,
        AttackSpeed,
        MovementSpeed,
        CritChance,
        CritMultiplier
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeModifiers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeModifiers()
    {
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            statMultipliers[statType] = 1.0f;
        }

        Debug.Log("Stat modifiers initialized");
    }

    public void SetModifier(StatType statType, float multiplier)
    {
        statMultipliers[statType] = multiplier;
        Debug.Log($"Set {statType} modifier to {multiplier}");
    }

    public void AddModifier(StatType statType, float additionalMultiplier)
    {
        if (!statMultipliers.ContainsKey(statType))
        {
            statMultipliers[statType] = 1.0f;
        }

        statMultipliers[statType] *= additionalMultiplier;
        Debug.Log($"Added to {statType} modifier, now {statMultipliers[statType]}");
    }

    public float GetModifier(StatType statType)
    {
        if (statMultipliers.ContainsKey(statType))
        {
            return statMultipliers[statType];
        }

        return 1.0f;
    }

    public void ResetModifier(StatType statType)
    {
        statMultipliers[statType] = 1.0f;
        Debug.Log($"Reset {statType} modifier to default");
    }

    public void ResetAllModifiers()
    {
        InitializeModifiers();
        Debug.Log("All stat modifiers reset");
    }
}