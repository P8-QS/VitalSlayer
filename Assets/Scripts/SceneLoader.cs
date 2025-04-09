using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindFirstObjectByType<SceneLoader>();

            if (_instance != null) return _instance;

            
            // DontDestroyOnLoad(_instance); // Make sure it persists
            return _instance;
        }
    }

    public Animator transition;
    public float transitionTime = 1f;
    private const string ManagersSceneName = "Managers";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject);
            EnsureManagersSceneLoaded(); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


    private void EnsureManagersSceneLoaded()
    {
        if (!IsSceneLoaded(ManagersSceneName))
        {
            SceneManager.LoadScene(ManagersSceneName, LoadSceneMode.Additive);
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
                return true;
        }
        return false;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithTransition(string sceneName, System.Action onLoaded = null)
    {
        StartCoroutine(TriggerTransitionAndLoadScene(sceneName, onLoaded));
    }

    private IEnumerator TriggerTransitionAndLoadScene(string sceneName, System.Action onLoaded)
    {
        transition?.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        if (onLoaded != null)
        {
            void Callback(Scene scene, LoadSceneMode mode)
            {
                onLoaded.Invoke();
                SceneManager.sceneLoaded -= Callback;
            }
            SceneManager.sceneLoaded += Callback;
        }

        SceneManager.LoadScene(sceneName);
    }
}
