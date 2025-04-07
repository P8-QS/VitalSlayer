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
            
            _instance = Resources.FindObjectsOfTypeAll<SceneLoader>()[0];
            
            if (_instance != null) return _instance;
            
            GameObject obj = new GameObject("SceneLoader");
            _instance = obj.AddComponent<SceneLoader>();
            return _instance;
        }
    }
    
    public Animator transition;
    
    public float transitionTime = 1f;
    
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
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        // Register scene load callback
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