using UnityEngine;
using System.IO;
using System.Text;
using System;

public class ProgressionReporter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int maxLevelToReport = 20;
    [SerializeField] private string fileName = "progression_data.txt";

    public void GenerateProgressionReport()
    {
        if (player == null || player.playerStats == null)
        {
            Debug.LogError("Player or PlayerStats not assigned!");
            return;
        }

        StringBuilder sb = new StringBuilder();

        // Add timestamp
        sb.AppendLine($"# Game Progression Data - {DateTime.Now}");
        sb.AppendLine();

        // Player current stats
        sb.AppendLine($"## Current Player Status (Level {ExperienceManager.Instance.Level})");
        sb.AppendLine();
        sb.AppendLine($"Current XP: {ExperienceManager.Instance.Experience}");
        sb.AppendLine($"XP Needed for Next Level: {ExperienceManager.Instance.ExperienceMax}");
        sb.AppendLine($"Current Health: {player.maxHitpoint}");
        sb.AppendLine($"Current Damage Range: {player.playerStats.CalculateMinDamage(ExperienceManager.Instance.Level)}-{player.playerStats.CalculateMaxDamage(ExperienceManager.Instance.Level)}");

        sb.AppendLine();
        sb.AppendLine("## Level Progression Table");
        sb.AppendLine();
        sb.AppendLine("| Level | XP Required | Cumulative XP | Player Health | Min Damage | Max Damage |");
        sb.AppendLine("|-------|------------|--------------|---------------|------------|------------|");

        int cumulativeXp = 0;

        for (int level = 1; level <= maxLevelToReport; level++)
        {
            // Calculate XP required to reach this level
            int xpRequired = CalculateXpRequired(level);

            // Add to cumulative XP
            if (level > 1)
                cumulativeXp += xpRequired;

            int playerHealth = player.playerStats.CalculateMaxHealth(level);
            int minDamage = player.playerStats.CalculateMinDamage(level);
            int maxDamage = player.playerStats.CalculateMaxDamage(level);

            sb.AppendLine($"| {level} | {xpRequired} | {cumulativeXp} | {playerHealth} | {minDamage} | {maxDamage} |");
        }

        // Create folder if it doesn't exist
        string directory = Path.Combine(Application.persistentDataPath, "Reports");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, sb.ToString());

        Debug.Log($"Progression report saved to: {filePath}");
    }

    private int CalculateXpRequired(int level)
    {
        // This should match the formula in ExperienceManager.cs
        if (level == 1) return 0;
        return 100 * (level - 1) + (int)Mathf.Pow(level - 1, 2) * 25;
    }

    // UI button can call this method
    public void OnGenerateReportButtonClick()
    {
        GenerateProgressionReport();
    }
}