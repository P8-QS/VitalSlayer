using Effects;
using Metrics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MetricCardUI : MonoBehaviour
{
    private IMetric cardMetric;
    public TextMeshProUGUI titleText; // Title
    public TextMeshProUGUI descriptionText; // Description
    public GameObject icons; // Reference to the icons
    public GameObject modal;
    public GameObject cardItemPrefab;
    public Transform modalParent;

    public AudioClip clickSound;

    // Method to set data dynamically
    public void SetMetric(IMetric metric)
    {
        cardMetric = metric;

        titleText.text = cardMetric.Name;
        descriptionText.text = cardMetric.Text();

        // Add icons dynamically, the metric icon should be added first
        // Instantiate image metric icon and attach to "icons" GameObject
        AddIcon(metric);
        foreach (var effect in metric.Effects)
        {
            AddIcon(effect, false);
        }
    }

    private void AddIcon(IMetric metric)
    {
        GameObject metricIconObject = new GameObject("Icon");
        metricIconObject.transform.SetParent(icons.transform);
        metricIconObject.transform.localScale = Vector3.one;
        var metricIconImage = metricIconObject.AddComponent<Image>();
        metricIconImage.sprite = metric.Icon;
    }

    private void AddIcon(IEffect effect, bool displayLevelIndicator)
    {
        GameObject metricIconObject = new GameObject("Icon");
        metricIconObject.transform.SetParent(icons.transform);
        metricIconObject.transform.localScale = Vector3.one;
        var metricIconImage = metricIconObject.AddComponent<Image>();
        metricIconImage.sprite = effect.Icon;

        if (effect.Level > 0 && displayLevelIndicator)
        {
            GameObject levelIndicatorBackground = new GameObject("LevelIndicatorBackground");
            levelIndicatorBackground.transform.SetParent(metricIconObject.transform);
            levelIndicatorBackground.transform.localScale = Vector3.one;
            var levelIndicatorBackgroundImage = levelIndicatorBackground.AddComponent<Image>();
            levelIndicatorBackgroundImage.color = Color.white;

            GameObject levelIndicatorObject = new GameObject("LevelIndicator");
            levelIndicatorObject.transform.SetParent(levelIndicatorBackground.transform);
            levelIndicatorObject.transform.localScale = Vector3.one;
            levelIndicatorObject.transform.localPosition = Vector3.zero;

            var levelIndicatorText = levelIndicatorObject.AddComponent<TextMeshProUGUI>();
            levelIndicatorText.text = $"{effect.Level}";
            levelIndicatorText.fontSize = 12;
            levelIndicatorText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MinecraftRegular SDF");
            levelIndicatorText.color = Color.black;
            levelIndicatorText.alignment = TextAlignmentOptions.Center;

            var levelTextSize = levelIndicatorText.GetPreferredValues();
            var backgroundRect = levelIndicatorBackground.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = new Vector2(levelTextSize.x + 2, levelTextSize.y);
            backgroundRect.localPosition = new Vector3(14, 12, 0);
        }
    }

    public void OnClick()
    {
        SoundFxManager.Instance.PlayClickSound();
        GameObject modalObject = Instantiate(modal, modalParent);
        ModalUI modalUI = modalObject.GetComponent<ModalUI>();

        // Create CardItems dynamically
        GameObject cardItem = Instantiate(cardItemPrefab, modalUI.content.transform);
        CardItemUI cardItemUI = cardItem.GetComponent<CardItemUI>();

        cardItemUI.SetCard(cardMetric.Icon, cardMetric.Name, cardMetric.Description());

        foreach (IEffect effect in cardMetric.Effects)
        {
            GameObject effectItem = Instantiate(cardItemPrefab, modalUI.content.transform);
            CardItemUI effectItemUI = effectItem.GetComponent<CardItemUI>();
            effectItemUI.SetCard(effect.Icon, effect.Name, effect.Description());
        }
    }
}