using TMPro;
using UnityEngine;

public class MainMenuPage : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject promptPrefab;
    public Transform promptParent;

    public AudioClip menuMusic;

    private float updateInterval = 1f;
    private float nextUpdateTime = 0f;

    void Start()
    {
        SoundFxManager.Instance.PlayClickSound();
        HandleXpCooldown();
        SoundFxManager.Instance.PlayMusic(menuMusic, 0.1f, true);

        if (StateManager.Instance.promptForEmail)
        {
            Instantiate(promptPrefab, promptParent);
        }
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            HandleXpCooldown();
            nextUpdateTime = Time.time + updateInterval;
        }
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
        if (ExperienceManager.Instance.BonusXpEnabled)
            SceneLoader.Instance.LoadScene("MetricsScreen");
        else
            GameManager.Instance.StartRound();
    }

    public void ClickMetricsButton()
    {
        SoundFxManager.Instance.PlayClickSound();
        SceneLoader.Instance.LoadScene("Metrics");
    }

    public void ClickPerksButton()
    {
        SoundFxManager.Instance.PlayClickSound();
        SceneLoader.Instance.LoadScene("Perks");
    }
}