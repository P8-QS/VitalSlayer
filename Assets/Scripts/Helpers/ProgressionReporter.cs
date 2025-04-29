using UnityEngine;
using System.IO;
using System.Text;
using System;

public class ProgressionReporter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int maxLevelToReport = 20;
    [SerializeField] private bool exportAsCSV = true;
    [SerializeField] private string fileName = "progression_data";

    public void GenerateProgressionReport()
    {
        if (player == null || player.playerStats == null)
        {
            Debug.LogError("Player or PlayerStats not assigned!");
            return;
        }

        // Create folder if it doesn't exist
        string directory = Path.Combine(Application.persistentDataPath, "Reports");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filePath;

        if (exportAsCSV)
        {
            filePath = Path.Combine(directory, $"{fileName}.csv");
            GenerateCSVReport(filePath);
        }
        else
        {
            filePath = Path.Combine(directory, $"{fileName}.txt");
            GenerateTextReport(filePath);
        }

        Debug.Log($"Progression report saved to: {filePath}");

        // Generate a simple current status text file regardless of format choice
        string statusPath = Path.Combine(directory, "current_status.txt");
        GenerateCurrentStatusReport(statusPath);
    }

    private void GenerateCSVReport(string filePath)
    {
        StringBuilder sb = new StringBuilder();

        // CSV header
        sb.AppendLine("Level,XP Required,Cumulative XP,Player Health,Min Damage,Max Damage");

        int cumulativeXp = 0;

        for (int level = 1; level <= maxLevelToReport; level++)
        {
            // Calculate XP required to reach this level
            int xpRequired = ExperienceManager.LevelToXpRequired(level);

            // Add to cumulative XP
            if (level > 1)
                cumulativeXp += xpRequired;

            int playerHealth = player.playerStats.CalculateMaxHealth(level);
            int minDamage = player.playerStats.CalculateMinDamage(level);
            int maxDamage = player.playerStats.CalculateMaxDamage(level);

            sb.AppendLine($"{level},{xpRequired},{cumulativeXp},{playerHealth},{minDamage},{maxDamage}");
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    private void GenerateTextReport(string filePath)
    {
        StringBuilder sb = new StringBuilder();

        // Table header with fixed width columns
        sb.AppendLine("Level  XP Required  Cumulative XP  Player Health  Min Damage  Max Damage");
        sb.AppendLine("-----  -----------  ------------  -------------  ----------  ----------");

        int cumulativeXp = 0;

        for (int level = 1; level <= maxLevelToReport; level++)
        {
            // Calculate XP required to reach this level
            int xpRequired = ExperienceManager.LevelToXpRequired(level);

            // Add to cumulative XP
            if (level > 1)
                cumulativeXp += xpRequired;

            int playerHealth = player.playerStats.CalculateMaxHealth(level);
            int minDamage = player.playerStats.CalculateMinDamage(level);
            int maxDamage = player.playerStats.CalculateMaxDamage(level);

            // Format with fixed width columns for better readability
            sb.AppendLine($"{level,-5}  {xpRequired,-11}  {cumulativeXp,-12}  {playerHealth,-13}  {minDamage,-10}  {maxDamage,-10}");
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    private void GenerateCurrentStatusReport(string filePath)
    {
        StringBuilder sb = new StringBuilder();

        // Add timestamp
        sb.AppendLine($"Game Status Report - {DateTime.Now}");
        sb.AppendLine("------------------------------------");
        sb.AppendLine();

        // Player current stats
        sb.AppendLine($"Current Player Level: {ExperienceManager.Instance.Level}");
        sb.AppendLine($"Current XP: {ExperienceManager.Instance.Experience}");
        sb.AppendLine($"XP Needed for Next Level: {ExperienceManager.Instance.ExperienceMax}");
        sb.AppendLine($"Current Health: {player.maxHitpoint}/{player.maxHitpoint}");
        sb.AppendLine($"Current Damage Range: {player.playerStats.CalculateMinDamage(ExperienceManager.Instance.Level)}-{player.playerStats.CalculateMaxDamage(ExperienceManager.Instance.Level)}");

        // Next level preview
        int nextLevel = ExperienceManager.Instance.Level + 1;
        sb.AppendLine();
        sb.AppendLine($"Next Level ({nextLevel}) Stats Preview:");
        sb.AppendLine($"Health: {player.playerStats.CalculateMaxHealth(nextLevel)}");
        sb.AppendLine($"Damage: {player.playerStats.CalculateMinDamage(nextLevel)}-{player.playerStats.CalculateMaxDamage(nextLevel)}");

        File.WriteAllText(filePath, sb.ToString());

        Debug.Log($"Current status report saved to: {filePath}");
    }

    // UI button can call this method
    public void OnGenerateReportButtonClick()
    {
        GenerateProgressionReport();
    }
}