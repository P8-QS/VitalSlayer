using UnityEngine;
using System;

public class ExperienceManager
{
    private static ExperienceManager _instance;
    public static ExperienceManager Instance => _instance ??= new ExperienceManager();

    private ExperienceManager(int xp = 0)
    {
        Experience = xp;

        if (PlayerPrefs.HasKey("Cooldown"))
        {
            CooldownEnd = DateTime.Parse(PlayerPrefs.GetString("Cooldown"));
        }
        else
        {
            CooldownEnd = DateTime.Now;
            PlayerPrefs.SetString("Cooldown", CooldownEnd.ToString());
        }
    }

    /// <summary>
    /// The bonus multiplier for experience.
    /// </summary>
    public readonly double BonusXpMultiplier = 1.5;

    /// <summary>
    /// The perk multiplier for experience.
    /// </summary>
    public double PerkXpMultiplier = 1.0;

    /// <summary>
    /// Whether the bonus multiplier is enabled.
    /// </summary>
    public bool BonusXpEnabled => GetXpCooldown().Milliseconds == 0;

    private int _experience;

    /// <summary>
    /// The current total experience.
    /// </summary>
    public int Experience
    {
        get => _experience;
        set
        {
            // Update experience
            _experience = value;

            // Update level if necessary
            Level = CalculateLevelFromTotalXp(value);
            ExperienceMax = LevelToXpRequired(Level);
        }
    }

    /// <summary>
    /// Experience required to reach next level.
    /// </summary>
    public int ExperienceMax { get; private set; }

    /// <summary>
    /// The current level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// The time when the bonus multiplier cooldown ends.
    /// </summary>
    public DateTime CooldownEnd;

    /// <summary>
    /// Calculates the level based on the total experience.
    /// </summary>
    /// <param name="totalXp"> Experience to calculate the level from.</param>
    /// <returns>The level.</returns>
    private static int CalculateLevelFromTotalXp(int totalXp)
    {
        var level = 1;
        while (LevelToXpRequired(level) <= totalXp) level++;
        return level;
    }

    /// <summary>
    /// Calculates the amount of experience required to reach the level.
    /// </summary>
    /// <param name="level">Level to reach.</param>
    /// <returns>Experience required to reach the level.</returns>
    public static int LevelToXpRequired(int level) => (int)(GameConstants.PLAYER_BASE_XP * Math.Pow(level, GameConstants.PLAYER_XP_SCALING_FACTOR)) * level;


    /// <summary>
    /// Adds experience and returns the amount of experience added after applying the bonus multiplier.
    /// </summary>
    /// <param name="xp">Amount of experience to add. </param>
    /// <returns> The experience added.</returns>
    public int AddExperience(int xp)
    {
        var adjustedXp = (int)(xp * PerkXpMultiplier * (BonusXpEnabled ? BonusXpMultiplier : 1));
        Experience += adjustedXp;
        return adjustedXp;
    }

    /// <summary>
    /// Adds game win experience.
    /// </summary>
    /// <returns>The experience added.</returns>
    public int AddGameWin() => AddExperience(GameConstants.WIN_XP * Level);

    /// <summary>
    /// Resets the cooldown for the bonus multiplier.
    /// The cooldown is set to the next day at 5 AM.
    /// </summary>
    public void ResetCooldown()
    {
        var now = DateTime.Now;
        var nextReset = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0);
        if (now > nextReset)
        {
            nextReset = nextReset.AddDays(1);
        }

        CooldownEnd = nextReset;
        PlayerPrefs.SetString("Cooldown", CooldownEnd.ToString());
    }

    /// <summary>
    /// Calculates the time remaining for the bonus multiplier cooldown.
    /// </summary>
    /// <returns> The time remaining for the cooldown. </returns>
    public TimeSpan GetXpCooldown() => CooldownEnd > DateTime.Now ? CooldownEnd - DateTime.Now : TimeSpan.Zero;
}