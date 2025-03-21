using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetricCardUI : MonoBehaviour
{
    private IMetric cardMetric;
    public TextMeshProUGUI titleText;  // Title
    public TextMeshProUGUI descriptionText;  // Description
    public GameObject icons;  // Reference to the icons
    public GameObject modal;
    public GameObject cardItemPrefab;
    public Transform modalParent;

    // Method to set data dynamically
    public void SetMetric(IMetric metric)
    {
        cardMetric = metric;

        titleText.text = cardMetric.Name;
        descriptionText.text = cardMetric.Text();
        
        // Add icons dynamically, the metric icon should be added first
        // Instantiate image metric icon and attach to "icons" GameObject
        AddIcon(metric.Icon);
        AddIcon(metric.Effect.Icon);    
    }

    private void AddIcon(Sprite icon)
    {
        GameObject metricIconObject = new GameObject("Icon");
        metricIconObject.transform.SetParent(icons.transform);
        metricIconObject.transform.localScale = Vector3.one;
        var metricIconImage = metricIconObject.AddComponent<Image>();
        metricIconImage.sprite = icon;
    }

    public void OnClick()
    {
        Debug.Log("Card clicked");
        GameObject modalObject = Instantiate(modal, modalParent);
        ModalUI modalUI = modalObject.GetComponent<ModalUI>();
        
        // Get icons
        // Image[] iconImages = icons.GetComponentsInChildren<Image>();
        
        // Create CardItems dynamically
        GameObject cardItem = Instantiate(cardItemPrefab, modalUI.content.transform);
        CardItemUI cardItemUI = cardItem.GetComponent<CardItemUI>();
        
        cardItemUI.SetCard(cardMetric.Icon, cardMetric.Name, cardMetric.Description());
        
        bool first = true;
        foreach (Transform icon in icons.transform)
        {
            if (first)
            {
                first = false;
                continue;
            }
            GameObject effectItem = Instantiate(cardItemPrefab, modalUI.content.transform);
            CardItemUI effectItemUI = effectItem.GetComponent<CardItemUI>();
            effectItemUI.SetCard(cardMetric.Effect.Icon, cardMetric.Effect.Name, cardMetric.Effect.Description());
        }
    }
}
