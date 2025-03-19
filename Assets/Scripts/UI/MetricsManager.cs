using System;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using Metrics;
using System.Collections.Generic;

public class MetricsManager : MonoBehaviour
{
    public GameObject metricCardPrefab;  // Assign the MetricCard prefab in Unity Inspector
    public Transform parentPanel;  // The UI Panel that holds all metric cards
    public Transform modalParent;  // The UI Panel that holds all modals

    void Start()
    {
        Debug.Log("MetricsManager is running!");

        List<IMetric> metrics = new List<IMetric>();

        StepsMetric stepsMetric = new StepsMetric();
        SleepMetric sleepMetric = new SleepMetric();
        ScreenTimeMetric screenTimeMetric = new ScreenTimeMetric();

        if (stepsMetric.Data != null) metrics.Add(stepsMetric);
        if (sleepMetric.Data != null) metrics.Add(sleepMetric);
        if (screenTimeMetric.Data != 0) metrics.Add(screenTimeMetric);

        bool hasMetrics = metrics.Count > 0;

        foreach (var metric in metrics)
        {
            AddMetric(metric);
        }

        if (!hasMetrics)
        {
            ShowErrorText("No metrics available.");
        }
    }

    void AddMetric(IMetric metric) {
        GameObject newCard = Instantiate(metricCardPrefab, parentPanel);
        MetricCardUI metricUI = newCard.GetComponent<MetricCardUI>();
        metricUI.modalParent = modalParent;
        metricUI.SetMetric(metric.Icon, metric.Name, metric.Description(), metric.Effect.Icon);
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
