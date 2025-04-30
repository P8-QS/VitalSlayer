using System;
using UnityEngine;

public class HealthPerk : IPerk
{
    public string Name => "Health";
    public int Cost { get; set; } = 1;
    public int Level { get; set; } = 0;

    public string Description()
    {
        if (Level == 0)
            return $"You will get <b>{GetMultiplier(Level + 1)}% more</b> health points.";

        return
            $"You will get <b>{GetMultiplier(Level + 1)}% more</b> health points if you upgrade. Currently you have <b>{GetMultiplier(Level)}%</b> more health points.";
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
        var multiplier = 1 + (GetMultiplier(Level) / 100.0);
        var baseHealth = player.playerStats.BaseHealth;
        player.playerStats.BaseHealth = (int)(baseHealth * multiplier);

        if (player.level > 0)
        {
            player.maxHitpoint = player.playerStats.CalculateMaxHealth(player.level);
            player.hitpoint = player.maxHitpoint;
        }
    }
}