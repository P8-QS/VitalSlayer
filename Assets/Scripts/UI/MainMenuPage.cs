using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenuPage : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject cooldownIcon;

    public AudioClip menuMusic;

    void Start()
    {
        SoundFxManager.Instance.PlayClickSound();
        HandleXpCooldown();
        SoundFxManager.Instance.PlayMusic(menuMusic, 0.1f, true);
    }

    void Update()
    {
        HandleXpCooldown();
    }

    private void HandleXpCooldown()
    {
        var xpBonus = ExperienceManager.Instance.BonusXpEnabled;
        var cooldown = ExperienceManager.Instance.GetXpCooldown();
        var bonusMultiplier = (int)((ExperienceManager.Instance.BonusXpMultiplier - 1) * 100);

        var readyString = "Play now to get " + bonusMultiplier + "% bonus XP!";
        var cooldownString = "You will get bonus XP in " + cooldown.ToString(@"hh\:mm\:ss") + ".";
        countdownText.text = xpBonus ? readyString : cooldownString;

        // TODO: Add icon for when bonus XP is ready
        // TODO: Center icon and text
        // cooldownIcon.gameObject.SetActive(!xpBonus);
    }

    public void ClickPlayButton()
    {
        SoundFxManager.Instance.PlayClickSound();
        GameManager.Instance.StartRound();
    }

    public void ClickMetricsButton()
    {
        SoundFxManager.Instance.PlayClickSound();
        SceneLoader.Instance.LoadScene("Metrics");
    }
}