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
        var metrics = MetricsManager.Instance?.metrics.Values;
        if (metrics != null) {
            foreach (IMetric metric in metrics) {
                GameObject effectIconInstance = Instantiate(effectIconPrefab, transform);
                effectIconInstance.GetComponent<Image>().sprite = metric.Effect.Icon;

                Button button = effectIconInstance.AddComponent<Button>();
                button.onClick.AddListener(() => ShowEffectDescription(metric.Effect.Description()));
            }
        } else {
            Debug.LogWarning("Metrics null in EffectUI");
        }
    }

    void ShowEffectDescription(string description) {
        effectDescriptionText.text = description;
        effectDescriptionText.gameObject.SetActive(true);
    }
}