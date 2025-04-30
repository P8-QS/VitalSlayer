using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuPrefab;
    public Transform pauseMenuParent;

    private GameObject pauseMenu;

    private static PauseMenuUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ClickPauseGame()
    {
        Time.timeScale = 0f; // Pause the game
        SoundFxManager.Instance.PlayClickSound();
        pauseMenu = Instantiate(pauseMenuPrefab, pauseMenuParent);
    }

    public void ClickQuitGame()
    {
        Time.timeScale = 1f; // Resume the game
        if (ExperienceManager.Instance.BonusXpEnabled) ExperienceManager.Instance.ResetCooldown();
        SceneLoader.Instance.LoadSceneWithTransition("MainMenu");
    }

    public void ClickResumeGame()
    {
        SoundFxManager.Instance.PlayClickSound();
        Destroy(gameObject);

        if (pauseMenu != null)
        {
            Destroy(pauseMenu);
        }

        Time.timeScale = 1f; // Resume the game
    }
}