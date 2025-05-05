using UnityEngine;


public class MetricsPage : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void ResetPerks()
    {
        PerksManager.Instance.ResetPerks();
    }
}
