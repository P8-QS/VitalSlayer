using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    private void Awake()
    {
        if (StateManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
    }

    public Player player;
    public FloatingTextManager floatingTextManager;


    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }


    public void SaveState()
    {
        Debug.Log("Saving game...");

        string s = "";

        s += "0" + "|";
        s += ExperienceManager.Instance.Experience + "|";
        s += "0";

        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        if (!PlayerPrefs.HasKey("SaveState"))
        {
            return;
        }

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        int experience = int.Parse(data[1]);
        ExperienceManager.Instance.Experience = experience;
        Debug.Log("Loading game...");
    }
}