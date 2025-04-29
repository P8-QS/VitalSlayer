using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        foreach (var effect in metrics.SelectMany(m => m.Effects))
        {
            // Instantiate effect icon inside EffectIconsContainer
            GameObject effectIconInstance = Instantiate(effectIconPrefab, transform);
            effectIconInstance.GetComponent<Image>().sprite = effect.Icon;

            // Add click listener to show effect description
            Button button = effectIconInstance.AddComponent<Button>();
            button.onClick.AddListener(() => ShowEffectDescription(effect.Description()));
        }

        effectDescriptionBox.SetActive(false);
    }

    void Update()
    {
        // Detect clicks while description box is open
        if (isDescriptionVisible &&
            Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame)
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

        // Scuffed! but position tracks the middle of the icon. You would then assume you could just offset
        // position x and y by half width and height, but nope that doesn't work properly.
        // Also the position relative to the icons somehow change depending on aspect ratio even though
        // we are directly offsetting from the icon position
        firstIconWorldPos.x -= firstIcon.GetComponent<RectTransform>().rect.width + 11;
        firstIconWorldPos.y -= firstIcon.GetComponent<RectTransform>().rect.height + 11;

        descBoxRect.position = firstIconWorldPos;

        effectDescriptionBox.SetActive(true);
        isDescriptionVisible = true;
    }

    public void HideEffectDescription()
    {
        effectDescriptionBox.SetActive(false);
        isDescriptionVisible = false;
    }
}