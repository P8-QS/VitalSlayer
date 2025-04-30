using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    public static PerksManager Instance;

    private int _points;

    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            StateManager.Instance.SaveState();
        }
    }

    public Dictionary<String, IPerk> Perks = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePerks();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePerks()
    {
        if (Instance == null)
        {
            Instance = this;
            // Add default perks
            var healthPerk = new HealthPerk();
            var experiencePerk = new ExperiencePerk();
            var attackSpeedPerk = new AttackSpeedPerk();
            var movementSpeedPerk = new MovementSpeedPerk();
            var criticalChancePerk = new CriticalChancePerk();
            var attackDamagePerk = new AttackDamagePerk();

            Perks.Add(healthPerk.Name, healthPerk);
            Perks.Add(experiencePerk.Name, experiencePerk);
            Perks.Add(attackSpeedPerk.Name, attackSpeedPerk);
            Perks.Add(movementSpeedPerk.Name, movementSpeedPerk);
            Perks.Add(criticalChancePerk.Name, criticalChancePerk);
            Perks.Add(attackDamagePerk.Name, attackDamagePerk);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Upgrade(IPerk perk)
    {
        if (!Perks.ContainsKey(perk.Name))
        {
            Debug.Log("Perk not found in the list.");
            return;
        }

        if (Points < perk.Cost)
        {
            Debug.Log("Not enough points to upgrade this perk.");
            return;
        }

        Points -= perk.Cost;
        perk.Upgrade();
        StateManager.Instance.SaveState();
    }

    public void AddPoints(int points)
    {
        Points += points;
    }

    /// <summary>
    /// Apply all perks to the current player
    /// Called when a new game starts
    /// </summary>
    public void ApplyPerksToPlayer()
    {
        if (!GameManager.Instance.player) return;

        GameManager.Instance.player.playerStats.Reset();

        foreach (var perk in Perks.Values)
        {
            if (perk.Level > 0)
            {
                perk.Apply();
            }
        }

        Debug.Log("Applied all perks to player");
        LogStatsBeforePerks();
        LogStatsAfterPerks();
    }

    #region Debug Methods

    private RuntimePlayerStats _beforeStats;
    private int _playerLevel;

    /// <summary>
    /// Logs player stats before perks are applied
    /// </summary>
    public void LogStatsBeforePerks()
    {
        if (GameManager.Instance?.player?.basePlayerStats == null) return;

        var player = GameManager.Instance.player;
        _playerLevel = player.level;

        _beforeStats = new RuntimePlayerStats(player.basePlayerStats);

        LogActivePerks();
    }

    /// <summary>
    /// Logs player stats after perks are applied and shows differences
    /// </summary>
    public void LogStatsAfterPerks()
    {
        if (GameManager.Instance?.player == null || _beforeStats == null) return;

        var player = GameManager.Instance.player;
        int level = player.level;

        // Get before values
        int beforeHealth = _beforeStats.CalculateMaxHealth(level);
        int beforeMinDamage = _beforeStats.CalculateMinDamage(level);
        int beforeMaxDamage = _beforeStats.CalculateMaxDamage(level);
        float beforeAttackCooldown = _beforeStats.AttackCooldown;
        float beforeSpeed = _beforeStats.BaseSpeed;
        float beforeCritChance = _beforeStats.CritChance;

        // Get after values
        int afterHealth = player.maxHitpoint;
        int afterMinDamage = player.playerStats.CalculateMinDamage(level);
        int afterMaxDamage = player.playerStats.CalculateMaxDamage(level);
        float afterAttackCooldown = player.playerStats.AttackCooldown;
        float afterSpeed = player.playerStats.BaseSpeed;
        float afterCritChance = player.playerStats.CritChance;

        // Calculate change percentages
        float healthChange = ((float)afterHealth / beforeHealth - 1f) * 100f;
        float minDamageChange = ((float)afterMinDamage / beforeMinDamage - 1f) * 100f;
        float maxDamageChange = ((float)afterMaxDamage / beforeMaxDamage - 1f) * 100f;
        float attackSpeedChange = (beforeAttackCooldown / afterAttackCooldown - 1f) * 100f;
        float moveSpeedChange = (afterSpeed / beforeSpeed - 1f) * 100f;
        float critChanceChange = (afterCritChance / beforeCritChance - 1f) * 100f;

        // Multipliers
        float healthMultiplier = player.playerStats.HealthMultiplier;
        float damageMultiplier = player.playerStats.DamageMultiplier;

        // Create formatted string for debug output
        string report = "====== PLAYER STATS (LEVEL " + level + ") ======\n" +
            $"Health:       {beforeHealth} → {afterHealth} ({healthChange:+0.0;-0.0;0}%)\n" +
            $"Min Damage:   {beforeMinDamage} → {afterMinDamage} ({minDamageChange:+0.0;-0.0;0}%)\n" +
            $"Max Damage:   {beforeMaxDamage} → {afterMaxDamage} ({maxDamageChange:+0.0;-0.0;0}%)\n" +
            $"Attack Speed: {1f / beforeAttackCooldown:F2}/s → {1f / afterAttackCooldown:F2}/s ({attackSpeedChange:+0.0;-0.0;0}%)\n" +
            $"Move Speed:   {beforeSpeed:F2} → {afterSpeed:F2} ({moveSpeedChange:+0.0;-0.0;0}%)\n" +
            $"Crit Chance:  {beforeCritChance * 100:F1}% → {afterCritChance * 100:F1}% ({critChanceChange:+0.0;-0.0;0}%)\n" +
            $"Health Mult:  x{healthMultiplier:F2}\n" +
            $"Damage Mult:  x{damageMultiplier:F2}\n" +
            "======================================";

        Debug.Log(report);
    }

    /// <summary>
    /// Logs details about each active perk
    /// </summary>
    private void LogActivePerks()
    {
        StringBuilder perkReport = new StringBuilder("====== ACTIVE PERKS ======\n");
        bool hasActivePerks = false;

        foreach (var perk in Perks.Values)
        {
            if (perk.Level > 0)
            {
                hasActivePerks = true;
                perkReport.AppendLine($"{perk.Name}: Level {perk.Level}");
            }
        }

        if (!hasActivePerks)
        {
            perkReport.AppendLine("No active perks");
        }

        perkReport.AppendLine("==========================");
        Debug.Log(perkReport.ToString());
    }

    /// <summary>
    /// Test utility to see the effect of a specific perk at a specific level
    /// </summary>
    public void TestPerk(string perkName, int testLevel)
    {
        if (!Perks.TryGetValue(perkName, out IPerk perk))
        {
            Debug.LogWarning($"Perk '{perkName}' not found");
            return;
        }

        if (GameManager.Instance?.player == null)
        {
            Debug.LogWarning("Cannot test perk - player not found");
            return;
        }

        // Store original level to restore later
        int originalLevel = perk.Level;

        // Reset player stats
        GameManager.Instance.player.playerStats.Reset();

        // Log before stats
        LogStatsBeforePerks();

        // Set test level
        perk.Level = testLevel;

        // Log before applying
        Debug.Log($"Testing {perkName} at level {testLevel}...");

        // Apply the perk
        perk.Apply();

        // Log the results
        LogStatsAfterPerks();

        // Restore original level
        perk.Level = originalLevel;

        // Reapply all perks at original levels
        ApplyPerksToPlayer();
    }

    /// <summary>
    /// Resets all perks to level 0 (for testing)
    /// </summary>
    public void ResetAllPerks()
    {
        if (GameManager.Instance?.player == null)
            return;

        // Reset player stats
        GameManager.Instance.player.playerStats.Reset();

        // Reset all perk levels to 0
        foreach (var perk in Perks.Values)
        {
            perk.Level = 0;
        }

        Debug.Log("All perks have been reset to level 0");

        // Save state
        StateManager.Instance.SaveState();
    }

    #endregion
}