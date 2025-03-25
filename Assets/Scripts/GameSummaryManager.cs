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
        xpStart = GameManager.instance.XpManager.Experience;
        levelStart = GameManager.instance.XpManager.Level;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEnemy()
    {
        enemiesKilled++;
    }

    public void AddBoss()
    {
        bossesKilled++;
    }
    
    
    public void Show(bool gameWon){
        Debug.Log("Game over!");
        GameObject go = Instantiate(gameSummaryPrefab, parentPanel);
        GameSummaryUI summaryUI = go.GetComponent<GameSummaryUI>();
        summaryUI.gameWon = gameWon;
        
        summaryUI.AddSummaryItem("Enemies slain", enemiesKilled.ToString());

        var xpManager = GameManager.instance.XpManager;
        int xpGained = xpManager.Experience - xpStart;
        int levelGained = xpManager.Level - levelStart;
        
        
        summaryUI.AddSummaryItem("Experience gained", xpManager.Level.ToString());
        summaryUI.AddSummaryItem("Level gained", xpManager.Experience.ToString());
    }
}
