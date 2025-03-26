using UnityEngine;

public class GameSummaryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject gameSummaryPrefab;
    public Transform parentPanel;

    public int enemiesKilled;
    public int bossesKilled;

    public int xpStart;
    public int levelStart;


    void Start()
    {
        xpStart = ExperienceManager.Instance.Experience;
        levelStart = ExperienceManager.Instance.Level;
    }

    public void AddEnemy()
    {
        enemiesKilled++;
    }

    public void AddBoss()
    {
        bossesKilled++;
    }


    public void Show()
    {
        Debug.Log("Game over!");
        GameObject go = Instantiate(gameSummaryPrefab, parentPanel);
        GameSummaryUI summaryUI = go.GetComponent<GameSummaryUI>();

        if (ExperienceManager.Instance.BonusXpEnabled) ExperienceManager.Instance.ResetCooldown();

        var gameWon = bossesKilled > 0;
        if (gameWon) ExperienceManager.Instance.AddGameWin();
        summaryUI.gameWon = gameWon;

        int totalEnemies = enemiesKilled + bossesKilled;

        summaryUI.AddSummaryItem("Enemies slain", totalEnemies.ToString());

        var xpManager = ExperienceManager.Instance;
        int xpGained = xpManager.Experience - xpStart;
        int levelGained = xpManager.Level - levelStart;

        summaryUI.AddSummaryItem("Experience gained", xpGained.ToString());
        summaryUI.AddSummaryItem("Levels gained", levelGained.ToString());
    }
}