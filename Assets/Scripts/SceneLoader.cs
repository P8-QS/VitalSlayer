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
    
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(TriggerTransitionAndLoadScene(sceneName));
    }

    
    private IEnumerator TriggerTransitionAndLoadScene(string sceneName)
    {
        // Trigger animation
        transition.SetTrigger("Start");

        // Wait for the animation to finish
        yield return new WaitForSeconds(transitionTime);
        
        SceneManager.LoadScene(sceneName);
    }
}