using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerksManagerUI : MonoBehaviour
{
    public GameObject perkCardPrefab; // Assign the MetricCard prefab in Unity Inspector
    public Transform parentPanel; // The UI Panel that holds all metric cards
    public TextMeshProUGUI pointsText;
    public GameObject toolTip;
    private List<PerkCardUI> perksCards = new();

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

        if (toolTip)
        {
            toolTip.SetActive(false);
        }
    }

    void Update()
    {
        pointsText.text = PerksManager.Instance.Points.ToString();
        foreach (var perk in perksCards)
        {
            perk.UpdatePerk();
        }

        if (toolTip && toolTip.activeSelf &&
            Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            HidePointsToolTip();
        }
    }

    public void OnPointsClick()
    {
        if (toolTip) toolTip.SetActive(true);
    }

    private void HidePointsToolTip()
    {
        if (toolTip)
        {
            toolTip.SetActive(false);
        }
    }

    void AddPerk(IPerk perk)
    {
        GameObject newCard = Instantiate(perkCardPrefab, parentPanel);
        PerkCardUI perkCardUI = newCard.GetComponent<PerkCardUI>();
        perkCardUI.SetPerk(perk);
        perksCards.Add(perkCardUI);
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