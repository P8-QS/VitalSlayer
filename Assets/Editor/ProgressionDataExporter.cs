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
            string extension = "csv";
            string defaultName = "progression_data.csv";
            outputPath = EditorUtility.SaveFilePanel("Save Progression Data", "", defaultName, extension);
        }

        EditorGUILayout.LabelField("Output Path:", outputPath);

        GUI.enabled = playerStats != null && !string.IsNullOrEmpty(outputPath);
        if (GUILayout.Button("Generate Progression Data"))
        {
            GenerateProgressionDataCSV();
        }
        GUI.enabled = true;
    }

    private void GenerateProgressionDataCSV()
    {
        if (playerStats == null || string.IsNullOrEmpty(outputPath))
        {
            EditorUtility.DisplayDialog("Error", "Please select a PlayerStats asset and output path", "OK");
            return;
        }

        StringBuilder sbPlayer = new StringBuilder();
        StringBuilder sbEnemy = new StringBuilder();
        StringBuilder sbBoss = new StringBuilder();

        sbPlayer.AppendLine("Level,XP Required,Cumulative XP,Player Health,Min Damage,Max Damage");

        int cumulativeXp = 0;

        for (int level = 1; level <= maxLevel; level++)
        {
            int xpRequired = CalculateXpRequired(level);
            cumulativeXp += xpRequired;

            int playerHealth = playerStats.CalculateMaxHealth(level);
            int minDamage = playerStats.CalculateMinDamage(level);
            int maxDamage = playerStats.CalculateMaxDamage(level);

            sbPlayer.AppendLine($"{level},{xpRequired},{cumulativeXp},{playerHealth},{minDamage},{maxDamage}");
        }

        BaseEnemyStats[] enemyStats = Resources.FindObjectsOfTypeAll<BaseEnemyStats>();
        BaseBossStats[] bossStats = Resources.FindObjectsOfTypeAll<BaseBossStats>();

        File.WriteAllText(outputPath, sbPlayer.ToString());

        if (enemyStats.Length > 0)
        {
            for (int i = 0; i < enemyStats.Length; i++)
            {
                BaseEnemyStats enemyStat = enemyStats[i];
                string enemyPath = Path.Combine(Path.GetDirectoryName(outputPath), $"enemy_{enemyStat.name.Replace(" ", "_")}.csv");

                sbEnemy.AppendLine("Level,Enemy Health,Min Damage,Max Damage,XP Reward");

                for (int level = 1; level <= maxLevel; level++)
                {
                    int enemyHealth = enemyStat.GetScaledHealth(level);
                    int minDamage = enemyStat.GetScaledMinDamage(level);
                    int maxDamage = enemyStat.GetScaledMaxDamage(level);
                    int xpReward = enemyStat.GetScaledXpReward(level);

                    sbEnemy.AppendLine($"{level},{enemyHealth},{minDamage},{maxDamage},{xpReward}");
                }

                File.WriteAllText(enemyPath, sbEnemy.ToString());
            }
        }

        if (bossStats.Length > 0)
        {
            BaseBossStats bossStat = bossStats[0];
            string bossPath = Path.Combine(Path.GetDirectoryName(outputPath), $"boss_{bossStat.name.Replace(" ", "_")}.csv");

            sbBoss.AppendLine("Level,Boss Health,Min Damage,Max Damage,XP Reward");

            for (int level = 1; level <= maxLevel; level++)
            {
                int bossHealth = bossStat.GetScaledHealth(level);
                int minDamage = bossStat.GetScaledMinDamage(level);
                int maxDamage = bossStat.GetScaledMaxDamage(level);
                int xpReward = bossStat.GetScaledXpReward(level);

                sbBoss.AppendLine($"{level},{bossHealth},{minDamage},{maxDamage},{xpReward}");
            }

            File.WriteAllText(bossPath, sbBoss.ToString());
        }

        EditorUtility.RevealInFinder(outputPath);
        Debug.Log($"Progression data exported to: {outputPath}");
    }

    private int CalculateXpRequired(int level)
    {
        return ExperienceManager.LevelToXpRequired(level);
    }
}

#endif