using UnityEngine;
using UnityEngine.SceneManagement;


public class MetricsPage : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }
}
