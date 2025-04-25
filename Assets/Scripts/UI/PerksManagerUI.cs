using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerksManagerUI : MonoBehaviour
{
    public GameObject perkCardPrefab; // Assign the MetricCard prefab in Unity Inspector
    public Transform parentPanel; // The UI Panel that holds all metric cards
    public TextMeshProUGUI pointsText;
    // public Transform modalParent; // The UI Panel that holds all modals

    void Start()
    {
        Debug.Log("PerksManagerUI is running!");

        var perks = PerksManager.Instance.Perks;

        foreach (var perk in perks)
        {
            AddPerk(perk.Value);
        }

        if (perks.Count < 0)
        {
            ShowErrorText("No perks available.");
        }
        
        pointsText.text = PerksManager.Instance.Points.ToString();
    }

    void Update()
    {
        pointsText.text = PerksManager.Instance.Points.ToString();
    }
    void AddPerk(IPerk perk)
    {
        GameObject newCard = Instantiate(perkCardPrefab, parentPanel);
        PerkCardUI perkCardUI = newCard.GetComponent<PerkCardUI>();
        perkCardUI.SetPerk(perk);
    }

    void ShowErrorText(string message)
    {
        GameObject go = new GameObject("ErrorText");
        go.transform.SetParent(parentPanel);
        var errorText = go.AddComponent<TextMeshProUGUI>();
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MinecraftRegular SDF");
        errorText.fontSize = 36;
        errorText.text = message;
    }
}