using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

#if UNITY_EDITOR
public class ProgressionDataExporter : EditorWindow
{
    private PlayerStats playerStats;
    private int maxLevel = 30;
    private string outputPath = "";

    [MenuItem("Tools/Export Progression Data")]
    public static void ShowWindow()
    {
        GetWindow<ProgressionDataExporter>("Progression Data Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Progression Data Export Settings", EditorStyles.boldLabel);
        
        playerStats = EditorGUILayout.ObjectField("Player Stats", playerStats, typeof(PlayerStats), false) as PlayerStats;
        maxLevel = EditorGUILayout.IntSlider("Max Level to Calculate", maxLevel, 1, 100);
        
        if (GUILayout.Button("Select Output Path"))
        {
            outputPath = EditorUtility.SaveFilePanel("Save Progression Data", "", "progression_data.txt", "txt");
        }

        EditorGUILayout.LabelField("Output Path:", outputPath);
        
        GUI.enabled = playerStats != null && !string.IsNullOrEmpty(outputPath);
        if (GUILayout.Button("Generate Progression Data"))
        {
            GenerateProgressionData();
        }
        GUI.enabled = true;
    }

    private void GenerateProgressionData()
    {
        if (playerStats == null || string.IsNullOrEmpty(outputPath))
        {
            EditorUtility.DisplayDialog("Error", "Please select a PlayerStats asset and output path", "OK");
            return;
        }

        StringBuilder sb = new StringBuilder();
        
        // Table header
        sb.AppendLine("# Game Progression Data");
        sb.AppendLine();
        sb.AppendLine("| Level | XP Required | Cumulative XP | Player Health | Min Damage | Max Damage |");
        sb.AppendLine("|-------|------------|--------------|---------------|------------|------------|");
        
        int cumulativeXp = 0;
        
        for (int level = 1; level <= maxLevel; level++)
        {
            int xpRequired = level == 1 ? 0 : CalculateXpRequired(level);
            cumulativeXp += level == 1 ? 0 : xpRequired;
            
            int playerHealth = playerStats.CalculateMaxHealth(level);
            int minDamage = playerStats.CalculateMinDamage(level);
            int maxDamage = playerStats.CalculateMaxDamage(level);
            
            sb.AppendLine($"| {level} | {xpRequired} | {cumulativeXp} | {playerHealth} | {minDamage} | {maxDamage} |");
        }
        
        sb.AppendLine();
        sb.AppendLine("# Enemy Scaling Example");
        sb.AppendLine();
        
        // Find enemy stats - just as an example
        BaseEnemyStats[] enemyStats = Resources.FindObjectsOfTypeAll<BaseEnemyStats>();
        BaseBossStats[] bossStats = Resources.FindObjectsOfTypeAll<BaseBossStats>();
        
        if (enemyStats.Length > 0)
        {
            BaseEnemyStats enemyStat = enemyStats[0];
            sb.AppendLine($"## Enemy: {enemyStat.name}");
            sb.AppendLine();
            sb.AppendLine("| Level | Enemy Health | Min Damage | Max Damage | XP Reward |");
            sb.AppendLine("|-------|-------------|------------|------------|-----------|");
            
            for (int level = 1; level <= maxLevel; level++)
            {
                int enemyHealth = enemyStat.GetScaledHealth(level);
                int minDamage = enemyStat.GetScaledMinDamage(level);
                int maxDamage = enemyStat.GetScaledMaxDamage(level);
                int xpReward = enemyStat.GetScaledXpReward(level);
                
                sb.AppendLine($"| {level} | {enemyHealth} | {minDamage} | {maxDamage} | {xpReward} |");
            }
            
            sb.AppendLine();
        }
        
        if (bossStats.Length > 0)
        {
            BaseBossStats bossStat = bossStats[0];
            sb.AppendLine($"## Boss: {bossStat.name}");
            sb.AppendLine();
            sb.AppendLine("| Level | Boss Health | Min Damage | Max Damage | XP Reward |");
            sb.AppendLine("|-------|------------|------------|------------|-----------|");
            
            for (int level = 1; level <= maxLevel; level++)
            {
                int bossHealth = bossStat.GetScaledHealth(level);
                int minDamage = bossStat.GetScaledMinDamage(level);
                int maxDamage = bossStat.GetScaledMaxDamage(level);
                int xpReward = bossStat.GetScaledXpReward(level);
                
                sb.AppendLine($"| {level} | {bossHealth} | {minDamage} | {maxDamage} | {xpReward} |");
            }
        }
        
        File.WriteAllText(outputPath, sb.ToString());
        EditorUtility.RevealInFinder(outputPath);
        
        Debug.Log($"Progression data exported to: {outputPath}");
    }
    
    private int CalculateXpRequired(int level)
    {
        // This should match the formula in ExperienceManager.cs
        return 100 * level + (int)Mathf.Pow(level, 2) * 25;
    }
}
#endif