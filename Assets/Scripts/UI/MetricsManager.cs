using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;

public class MetricsManager : MonoBehaviour
{
    public GameObject metricCardPrefab;  // Assign the MetricCard prefab in Unity Inspector
    public Transform parentPanel;  // The UI Panel that holds all metric cards
    public Transform modalParent;  // The UI Panel that holds all modals

    public SpriteAtlas spriteAtlas;
    void Start()
    {

        Sprite sleepIcon = spriteAtlas.GetSprite("metric_sleep");
        Sprite stepsIcon = spriteAtlas.GetSprite("metric_steps");
        Sprite screenTimeIcon = spriteAtlas.GetSprite("metric_screen_time");

        Sprite fogIcon = spriteAtlas.GetSprite("effect_fog");

        
        Debug.Log("MetricsManager is running!");

        // Example of populating cards dynamically
        AddMetric(sleepIcon, "Sleep", "You have slept 3 hours and 46 minutes.\nThis gives you hallucination 2.");
        AddMetric(stepsIcon, "Steps", "You have taken 11,463 steps. This gives you increased map size.");
        AddMetric(screenTimeIcon, "Screen Time", "You have spent 4 hours and 32 minutes on your phone.", new []{fogIcon});
        
        // Add error text if no metrics are available
        GameObject go = new GameObject("ErrorText");
        go.transform.SetParent(parentPanel);
        var errorText = go.AddComponent<TextMeshProUGUI>();
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MinecraftRegular SDF");
        errorText.fontSize = 36;
        errorText.text = "No metrics available.";
    }

    void AddMetric(Sprite icon, string title, string description, [CanBeNull] Sprite[] effectIcons = null)
    {
        GameObject newCard = Instantiate(metricCardPrefab, parentPanel);
        MetricCardUI metricUI = newCard.GetComponent<MetricCardUI>();
        metricUI.modalParent = modalParent;
        metricUI.SetMetric(icon, title, description, effectIcons);
    }
}
