using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenuPage : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    private const string baseString = "You will get bonus XP in ";
    private const string readyString = "Play now to get bonus XP.";
    public GameObject cooldownIcon;
    void Start()
    {
        HandleXpCooldown();
    }
    
    void Update()
    {
        HandleXpCooldown();
    }
    private void HandleXpCooldown()
    {
        var xpBonus = ExperienceManager.Instance.BonusXpEnabled;
        Debug.Log("MENU BONUS: " + xpBonus);
        var cooldown = ExperienceManager.Instance.GetXpCooldown();
        var text = xpBonus ?  readyString : baseString + cooldown.ToString(@"hh\:mm\:ss") + ".";
        countdownText.text = text;
        cooldownIcon.gameObject.SetActive(!xpBonus);
        
    }
    
    public void ClickPlayButton()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    
    public void ClickMetricsButton()
    {
        SceneManager.LoadScene("Metrics", LoadSceneMode.Single);
    }
}
