using System;
using UnityEngine;

public class GameSummaryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject gameSummaryPrefab;
    public Transform parentPanel;

    public DateTime roundStartTime;

    public int enemiesKilled;
    public int bossesKilled;

    public int xpStart;
    public int levelStart;

    public static GameSummaryManager Instance;
    
    public AudioClip gameWonSound;
    public AudioClip gameLoseSound;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


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
    
    public void Reset()
    {
        enemiesKilled = 0;
        bossesKilled = 0;
        xpStart = ExperienceManager.Instance.Experience;
        levelStart = ExperienceManager.Instance.Level;
    }


    public void Show()
    {
        Debug.Log("Game over!");
        GameObject go = Instantiate(gameSummaryPrefab, parentPanel);
        GameSummaryUI summaryUI = go.GetComponent<GameSummaryUI>();

        if (ExperienceManager.Instance.BonusXpEnabled) ExperienceManager.Instance.ResetCooldown();

        var gameWon = bossesKilled > 0;
        if (gameWon)
        {
            ExperienceManager.Instance.AddGameWin();
            SoundFxManager.Instance.PlaySound(gameWonSound, 1f);
        }
        else
        {
            SoundFxManager.Instance.PlaySound(gameLoseSound, 1f);
        }
        summaryUI.gameWon = gameWon;

        int totalEnemies = enemiesKilled + bossesKilled;

        summaryUI.AddSummaryItem("Enemies slain", totalEnemies.ToString());
        
        int xpGained = ExperienceManager.Instance.Experience - xpStart;
        int levelGained = ExperienceManager.Instance.Level - levelStart;

        if (xpGained > 0) summaryUI.AddSummaryItem("Experience gained", xpGained.ToString());
        if (levelGained > 0) summaryUI.AddSummaryItem("Levels gained", levelGained.ToString());
        
        LoggingManager.Instance.LogGameSummary(gameWon, totalEnemies, xpGained,  levelGained, roundStartTime);

        // Reset the summary
        Reset();
    }
}