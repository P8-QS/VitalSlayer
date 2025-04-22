using UnityEngine;
using Metrics;
using System.Collections.Generic;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }
    public Dictionary<string, IMetric> metrics = new Dictionary<string, IMetric>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            StepsMetric stepsMetric = new StepsMetric();
            SleepMetric sleepMetric = new SleepMetric();
            ScreenTimeMetric screenTimeMetric = new ScreenTimeMetric();

            if (stepsMetric.Data != null) metrics.Add(stepsMetric.Name, stepsMetric);
            if (sleepMetric.Data != null) metrics.Add(sleepMetric.Name, sleepMetric);
            if (screenTimeMetric.Data != 0) metrics.Add(screenTimeMetric.Name, screenTimeMetric);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}