using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;

    private void Awake()
    {
        if (StateManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
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