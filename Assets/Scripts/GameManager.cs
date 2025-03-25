using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake(){
        if (GameManager.instance != null){
            Destroy(gameObject);
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
    }

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> xpTable;

    public Player player;
    public FloatingTextManager floatingTextManager;

    public GameObject gameSummaryPrefab;
    public Transform summaryPanel;

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration){
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    public ExperienceManager XpManager = new ExperienceManager(0);

    public void SaveState(){
        Debug.Log("Saving game...");
     
        string s = "";

        s += "0" + "|";
        s += XpManager.Experience + "|";
        s += "0";

        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene s, LoadSceneMode mode){
        
        if(!PlayerPrefs.HasKey("SaveState")){
            return;
        }

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        int experience = int.Parse(data[1]);
        XpManager.Experience = experience;

        Debug.Log("Loading game...");
    }
    
    public void ShowGameSummary(bool gameWon){
        Debug.Log("Game over!");
        GameObject go = Instantiate(gameSummaryPrefab, summaryPanel);
        GameSummary summary = go.GetComponent<GameSummary>();
        summary.gameWon = gameWon;
        
        summary.AddSummaryItem("Current level", XpManager.Level.ToString());
        summary.AddSummaryItem("Current experience", XpManager.Experience.ToString());
    }
    

}
