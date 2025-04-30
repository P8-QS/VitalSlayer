using System;
using UnityEngine;

public class ExperiencePerk : IPerk
{
    public string Name => "XP Bonus";
    public int Cost { get; set; } = 1;
    public int Level { get; set; } = 0;

    public string Description()
    {
        if (Level == 0)
            return $"You will get <b>{GetMultiplier(Level + 1)}% more</b> XP points.";
        return
            $"You will get <b>{GetMultiplier(Level + 1)}% more</b> XP points if you upgrade. Currently you have <b>{GetMultiplier(Level)}%</b> more XP points.";
    }

    private int GetMultiplier(int level)
    {
        return level * 10;
    }

    public void Apply()
    {
        if (ExperienceManager.Instance == null)
            return;

        var multiplier = 1 + (GetMultiplier(Level) / 100.0);
        ExperienceManager.Instance.PerkXpMultiplier = multiplier;
    }

    public void Upgrade()
    {
        Level++;
        Cost = (int)Math.Pow(Level + 1, 2);
    }
}