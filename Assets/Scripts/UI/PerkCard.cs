using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkCardUI : MonoBehaviour
{
    private IPerk perk;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Sprite enabledIcon;
    public Sprite disabledIcon;
    public GameObject upgradeButton;

    // Method to set data dynamically
    public void SetPerk(IPerk perk)
    {
        this.perk = perk;
        titleText.text = perk.Name;
        descriptionText.text = perk.Description();
        costText.text = perk.Cost.ToString();
        UpdatePerkButton();
    }

    private void UpdatePerkButton()
    {
        var buttonImage = upgradeButton.GetComponent<Image>();
        var button = upgradeButton.GetComponent<Button>();
        

        if (perk.Cost > PerksManager.Instance.Points)
        {
            button.interactable = false;
            buttonImage.sprite = disabledIcon;
        }
        else if (perk.Cost <= PerksManager.Instance.Points)
        {
            button.interactable = true;
            buttonImage.sprite = enabledIcon;
        }
    }

    public void UpdatePerk()
    {
        if (perk == null) return;

        titleText.text = perk.Name;
        descriptionText.text = perk.Description();
        costText.text = perk.Cost.ToString();
        UpdatePerkButton();
    }

    public void OnClick()
    {
        SoundFxManager.Instance.PlayClickSound();
        PerksManager.Instance.Upgrade(perk);
        UpdatePerk();
    }
}