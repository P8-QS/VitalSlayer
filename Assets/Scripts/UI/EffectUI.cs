using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Metrics;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EffectUI : MonoBehaviour
{
    public GameObject effectIconPrefab; // Prefab for effect icons
    public GameObject effectDescriptionBox; // Floating description box
    public TextMeshProUGUI effectDescriptionText; // Text inside the description box

    private bool isDescriptionVisible = false;

    void Start()
    {
        var metrics = MetricsManager.Instance.metrics.Values;
        foreach (IMetric metric in metrics)
        {
            // Instantiate effect icon inside EffectIconsContainer
            GameObject effectIconInstance = Instantiate(effectIconPrefab, transform);
            effectIconInstance.GetComponent<Image>().sprite = metric.Effect.Icon;

            // Add click listener to show effect description
            Button button = effectIconInstance.AddComponent<Button>();
            button.onClick.AddListener(() => ShowEffectDescription(metric.Effect.Description()));
        }

        effectDescriptionBox.SetActive(false);
    }
    
    void Update()
    {
        // Detect clicks while description box is open
        if (isDescriptionVisible && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            HideEffectDescription();
        }
    }

    void ShowEffectDescription(string description)
    {
        effectDescriptionText.text = description;
        
        // Position description box near icons
        RectTransform descBoxRect = effectDescriptionBox.GetComponent<RectTransform>();
        GameObject firstIcon = transform.GetChild(0).gameObject; 
        Vector3 firstIconWorldPos = firstIcon.transform.position;
        firstIconWorldPos.x -= firstIcon.GetComponent<RectTransform>().rect.width / 2;
        firstIconWorldPos.y -= firstIcon.GetComponent<RectTransform>().rect.height / 2;
        
        Vector3 localPos = descBoxRect.parent.InverseTransformPoint(firstIconWorldPos);
        descBoxRect.localPosition = localPos;

        effectDescriptionBox.SetActive(true);
        isDescriptionVisible = true;
    }

    public void HideEffectDescription()
    {
        Debug.Log("I AM CALLED!");
        effectDescriptionBox.SetActive(false);
        isDescriptionVisible = false;
    }
}
