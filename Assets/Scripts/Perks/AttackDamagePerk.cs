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
        if (GameManager.Instance == null || GameManager.Instance.player == null ||
            GameManager.Instance.player.playerStats == null)
            return;

        var player = GameManager.Instance.player;
        var multiplier = 1 + (GetMultiplier(Level) / 100.0);

        player.playerStats.DamageMultiplier = (float)multiplier;
    }
}