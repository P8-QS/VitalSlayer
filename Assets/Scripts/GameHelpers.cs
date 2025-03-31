using UnityEngine;

public static class GameHelpers
{
    private static System.Random random = new System.Random();

    public static int CalculateDamage(int minDamage, int maxDamage, float critChance = 0, float critMultiplier = 1)
    {
        int damage = random.Next(minDamage, maxDamage + 1);

        if (critChance > 0 && random.NextDouble() < critChance)
        {
            damage = Mathf.RoundToInt(damage * critMultiplier);
            return damage;
        }

        return damage;
    }

    public static int CalculateDamageStat(int baseDamage, int level, float scalingFactor)
    {
        if (level <= 0)
        {
            return baseDamage;
        }

        return baseDamage + (int)(baseDamage * (Mathf.Pow(level - 1, scalingFactor) * 0.1f));
    }

}