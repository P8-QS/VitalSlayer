using System.Collections.Generic;
using Metrics;
using UnityEngine;

namespace Managers
{
    public class MetricsManager : MonoBehaviour
    {
        public static MetricsManager Instance { get; private set; }
        public Dictionary<string, IMetric> metrics = new();

        public void Start()
        {
            if (Instance == null)
            {
                Instance = this;

                var stepsMetric = new StepsMetric();
                var sleepMetric = new SleepMetric();
                var exerciseMetric = new ExerciseMetric();
                var screenTimeMetric = new ScreenTimeMetric();
                var vo2MaxMetric = new Vo2MaxMetric();
                var activeCaloriesMetric = new ActiveCaloriesMetric();

                if (stepsMetric.Data != null) metrics.Add(stepsMetric.Name, stepsMetric);
                if (sleepMetric.Data != null) metrics.Add(sleepMetric.Name, sleepMetric);
                if (exerciseMetric.Data != null) metrics.Add(exerciseMetric.Name, exerciseMetric);
                if (screenTimeMetric.Data != 0) metrics.Add(screenTimeMetric.Name, screenTimeMetric);
                if (vo2MaxMetric.Data != null) metrics.Add(vo2MaxMetric.Name, vo2MaxMetric);
                if (activeCaloriesMetric.Data != null) metrics.Add(activeCaloriesMetric.Name, activeCaloriesMetric);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}