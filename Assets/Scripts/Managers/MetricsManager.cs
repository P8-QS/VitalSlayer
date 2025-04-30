using System.Collections.Generic;
using Data;
using Data.Models;
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
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (UserMetricsHandler.Instance != null)
            {
                UserMetricsHandler.Instance.OnStepsRecordsUpdated += OnStepsUpdated;
                UserMetricsHandler.Instance.OnSleepSessionRecordsUpdated += OnSleepUpdated;
                UserMetricsHandler.Instance.OnExerciseSessionRecordsUpdated += OnExerciseUpdated;
                UserMetricsHandler.Instance.OnTotalScreenTimeUpdated += OnScreenTimeUpdated;
                UserMetricsHandler.Instance.OnVo2MaxRecordsUpdated += OnVo2MaxUpdated;
                UserMetricsHandler.Instance.OnActiveCaloriesBurnedRecordsUpdated += OnActiveCaloriesBurnedUpdated;
                UserMetricsHandler.Instance.OnHeartRateVariabilityRecordsUpdated += OnHeartRateVariabilityUpdated;
            }
        }

        private void OnDisable()
        {
            if (UserMetricsHandler.Instance != null)
            {
                UserMetricsHandler.Instance.OnStepsRecordsUpdated -= OnStepsUpdated;
                UserMetricsHandler.Instance.OnSleepSessionRecordsUpdated -= OnSleepUpdated;
                UserMetricsHandler.Instance.OnExerciseSessionRecordsUpdated -= OnExerciseUpdated;
                UserMetricsHandler.Instance.OnTotalScreenTimeUpdated -= OnScreenTimeUpdated;
                UserMetricsHandler.Instance.OnVo2MaxRecordsUpdated -= OnVo2MaxUpdated;
                UserMetricsHandler.Instance.OnActiveCaloriesBurnedRecordsUpdated -= OnActiveCaloriesBurnedUpdated;
                UserMetricsHandler.Instance.OnHeartRateVariabilityRecordsUpdated -= OnHeartRateVariabilityUpdated;
            }
        }

        private void OnStepsUpdated(IReadOnlyCollection<StepsRecord> newSteps)
        {
            var stepsMetric = new StepsMetric();
            metrics.Remove(stepsMetric.Name);
            if (stepsMetric.Data != null)
            {
                metrics.Add(stepsMetric.Name, stepsMetric);
            }
        }

        private void OnScreenTimeUpdated(long newScreenTime)
        {
            var screenTimeMetric = new ScreenTimeMetric();
            metrics.Remove(screenTimeMetric.Name);
            if (screenTimeMetric.Data != 0)
            {
                metrics.Add(screenTimeMetric.Name, screenTimeMetric);
            }
        }

        private void OnSleepUpdated(IReadOnlyCollection<SleepSessionRecord> newSleep)
        {
            var sleepMetric = new SleepMetric();
            metrics.Remove(sleepMetric.Name);
            if (sleepMetric.Data != null)
            {
                metrics.Add(sleepMetric.Name, sleepMetric);
            }
        }

        private void OnExerciseUpdated(IReadOnlyCollection<ExerciseSessionRecord> newExercise)
        {
            var exerciseMetric = new ExerciseMetric();
            metrics.Remove(exerciseMetric.Name);
            if (exerciseMetric.Data != null)
            {
                metrics.Add(exerciseMetric.Name, exerciseMetric);
            }
        }

        private void OnVo2MaxUpdated(IReadOnlyCollection<Vo2MaxRecord> newVo2Max)
        {
            var vo2MaxMetric = new Vo2MaxMetric();
            metrics.Remove(vo2MaxMetric.Name);
            if (vo2MaxMetric.Data != null)
            {
                metrics.Add(vo2MaxMetric.Name, vo2MaxMetric);
            }
        }

        private void OnActiveCaloriesBurnedUpdated(
            IReadOnlyCollection<ActiveCaloriesBurnedRecord> newActiveCaloriesBurned)
        {
            var activeCaloriesMetric = new ActiveCaloriesMetric();
            metrics.Remove(activeCaloriesMetric.Name);
            if (activeCaloriesMetric.Data != null)
            {
                metrics.Add(activeCaloriesMetric.Name, activeCaloriesMetric);
            }
        }

        private void OnHeartRateVariabilityUpdated(IReadOnlyCollection<HeartRateVariabilityRmssdRecord> newHRV)
        {
            var hrvMetric = new HeartRateVariabilityMetric();
            metrics.Remove(hrvMetric.Name);
            if (hrvMetric.AvgHrvRmssd > 0)
            {
                metrics.Add(hrvMetric.Name, hrvMetric);
            }
        }
    }
}