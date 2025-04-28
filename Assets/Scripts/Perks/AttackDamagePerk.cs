using System;
using UnityEngine;

public class AttackDamagePerk : IPerk
{
    public string Name => "Attack Damage";
    public int Cost { get; set; } = 1;
    public int Level { get; set; } = 0;

    public string Description()
    {
        if (Level == 0)
            return $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> attack damage.";

        return
            $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> attack damage if you upgrade. Currently you have <b>{GetMultiplier(Level)}%</b> increase.";
    }

    private int GetMultiplier(int level)
    {
        return level * 2;
    }

    public void Upgrade()
    {
        Level++;
        Cost++;
    }

    public void Apply()
    {
        var baseFactor = GameManager.Instance.player.playerStats.damageScalingFactor;
        var multiplier = 1 + (GetMultiplier(Level) / 100.0);
        float newFactor = (float)(baseFactor * multiplier);
        GameManager.Instance.player.playerStats.baseSpeed = newFactor;
    }
}