using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPage : MonoBehaviour
{
    public bool xpReady = false;
    public TimeSpan cooldown;
    public TextMeshProUGUI countdownText;
    private DateTime cooldownEnd;
    private const string baseString = "You can gain XP in ";
    private const string readyString = "Play now to gain XP.";
    public GameObject cooldownIcon;
    void Start()
    {
        // Get cooldown DateTime from PlayerPrefs
        if (PlayerPrefs.HasKey("Cooldown"))
        {
            cooldownEnd = DateTime.Parse(PlayerPrefs.GetString("Cooldown"));
        }
        else
        {
            cooldownEnd = DateTime.Now.AddSeconds(11);
            PlayerPrefs.SetString("Cooldown", cooldownEnd.ToString());
        }
        
        cooldown = cooldownEnd - DateTime.Now;
        countdownText.text = GetDisplayString();
        xpReady = cooldown.TotalMilliseconds == 0;
        PlayerPrefs.SetInt("XpReady", xpReady ? 1 : 0);
        
    }
    
    private string GetDisplayString()
    {
        return xpReady ? readyString : baseString + cooldown.ToString(@"hh\:mm\:ss") + ".";
    }

    public void ResetCooldown()
    {
        xpReady = false;
        cooldownEnd = DateTime.Now.AddDays(1);
        PlayerPrefs.SetString("Cooldown", cooldownEnd.ToString());
        PlayerPrefs.SetInt("XpReady", xpReady ? 1 : 0);
    }
    
    // Update is called once per frame
    void Update()
    {
        cooldown = cooldownEnd > DateTime.Now ? cooldownEnd - DateTime.Now : TimeSpan.Zero;
        xpReady = cooldown.TotalMilliseconds == 0;
        countdownText.text = GetDisplayString();
        cooldownIcon.gameObject.SetActive(!xpReady);
        PlayerPrefs.SetInt("XpReady", xpReady ? 1 : 0);
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
