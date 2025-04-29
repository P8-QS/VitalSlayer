using System;
using UnityEngine;

public class MovementSpeedPerk : IPerk
{
    public string Name => "Movement Speed";
    public int Cost { get; set; } = 1;
    public int Level { get; set; } = 0;

    public string Description()
    {
        if (Level == 0)
            return $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> movement speed.";

        return
            $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> movement speed if you upgrade. Currently you have <b>{GetMultiplier(Level)}%</b> increase.";
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
        var baseSpeed = GameManager.Instance.player.playerStats.baseSpeed;
        var multiplier = 1 + (GetMultiplier(Level) / 100.0);
        float newSpeed = (float)(baseSpeed * multiplier);
        GameManager.Instance.player.playerStats.baseSpeed = newSpeed;
    }
}