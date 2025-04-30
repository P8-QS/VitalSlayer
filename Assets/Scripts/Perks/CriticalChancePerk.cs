using System;
using UnityEngine;

public class CriticalChancePerk : IPerk
{
    public string Name => "Critical Strike Chance";
    public int Cost { get; set; } = 1;
    public int Level { get; set; } = 0;

    public string Description()
    {
        if (Level == 0)
            return $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> critical strike chance.";

        return
            $"You will get <b>{GetMultiplier(Level + 1)}% increased</b> critical strike chance if you upgrade. Currently you have <b>{GetMultiplier(Level)}%</b> increase.";
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
        if (GameManager.Instance == null || GameManager.Instance.player == null ||
            GameManager.Instance.player.playerStats == null)
            return;

        var player = GameManager.Instance.player;
        var baseCrit = player.playerStats.CritChance;
        var multiplier = 1 + (GetMultiplier(Level) / 100.0);
        float newCrit = (float)(baseCrit * multiplier);
        player.playerStats.CritChance = newCrit;
    }
}