using UnityEngine;

public static class GameHelpers
{
    private static System.Random random = new System.Random();

    /// <summary>
    /// Calculates damage based on a range and applies critical hit chance and multiplier.
    /// </summary>
    /// <param name="minDamage"></param>
    /// <param name="maxDamage"></param>
    /// <param name="critChance"></param>
    /// <param name="critMultiplier"></param>
    /// <returns></returns>
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

    public static int CalculateDamageStat(int baseStat, int level, float scalingFactor, float multiplier = 1.0f)
    {
        if (level <= 0)
        {
            return Mathf.RoundToInt(baseStat * multiplier);
        }

        int calculatedStat = baseStat + (int)Mathf.Pow(level - 1, scalingFactor);
        return Mathf.RoundToInt(calculatedStat * multiplier);
    }

}