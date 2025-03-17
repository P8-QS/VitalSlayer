using UnityEngine;
using UnityEngine.SceneManagement;


public class MetricsPage : MonoBehaviour
{
    public void BackToMenu()
    {
        Debug.Log("Back to the lobby!");
        SceneManager.LoadScene("MainMenu");
    }
}
