using System.Linq;
using Managers;
using UnityEngine;
using TMPro;
using Metrics;

namespace UI
{
    public class MetricsManagerUI : MonoBehaviour
    {
        public GameObject metricCardPrefab; // Assign the MetricCard prefab in Unity Inspector
        public Transform parentPanel; // The UI Panel that holds all metric cards
        public Transform modalParent; // The UI Panel that holds all modals

        private void Start()
        {
            Debug.Log("MetricsManagerUI is running!");

            var metrics = MetricsManager.Instance.metrics.Values;
            
            foreach (var metric in metrics)
            {
                AddMetric(metric);
            }

            if (metrics.All(m => m.Name != "Screen Time"))
            {
                ShowErrorText("Screen Time metric is not available. You need to give QSCrawler access to Usage data.");
            }

            if ((metrics.Any(m => m.Name == "Screen Time") && metrics.Count == 1) || metrics.Count == 0)
            {
                ShowErrorText("No health metrics available. You need to give QSCrawler access to Health Connect.");
            }
        }

        void AddMetric(IMetric metric)
        {
            GameObject newCard = Instantiate(metricCardPrefab, parentPanel);
            MetricCardUI metricUI = newCard.GetComponent<MetricCardUI>();
            metricUI.modalParent = modalParent;
            metricUI.SetMetric(metric);
        }

        void ShowErrorText(string message)
        {
            GameObject go = new GameObject("ErrorText");
            go.transform.SetParent(parentPanel);
            var errorText = go.AddComponent<TextMeshProUGUI>();
            errorText.alignment = TextAlignmentOptions.Center;
            errorText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MinecraftRegular SDF");
            errorText.fontSize = 30;
            errorText.text = message;
        }
    }
}