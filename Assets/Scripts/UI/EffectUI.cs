using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public GameObject effectIconPrefab;
    public TextMeshProUGUI effectDescriptionText;
    public void Start()
    {
        var metrics = MetricsManager.Instance.metrics.Values;
        foreach (IMetric metric in metrics) {
            GameObject effectIconInstance = Instantiate(effectIconPrefab, transform);
            effectIconInstance.GetComponent<Image>().sprite = metric.Effect.Icon;

            Button button = effectIconInstance.AddComponent<Button>();
            button.onClick.AddListener(() => ShowEffectDescription(metric.Effect.Description()));
        }
    }

    void ShowEffectDescription(string description) {
        effectDescriptionText.text = description;
        effectDescriptionText.gameObject.SetActive(true);
    }
}