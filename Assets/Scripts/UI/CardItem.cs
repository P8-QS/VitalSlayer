using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardItemUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;  // Title
    public TextMeshProUGUI descriptionText;  // Description
    public Image icon;

    // Method to set data dynamically
    public void SetCard(Sprite cardIcon, string title, string description)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (icon != null) icon.sprite = cardIcon;
    }
}