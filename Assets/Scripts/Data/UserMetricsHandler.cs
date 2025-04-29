using System;
using System.Collections.Generic;
using System.IO;
using Data.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public enum UserMetricsType
    {
        ActiveCaloriesBurned,
        ExerciseSessionRecords,
        HeartRateVariabilityRmssd,
        StepsRecords,
        SleepSessionRecords,
        TotalScreenTime,
        Vo2Max
    }
    
    public class UserMetricsHandler : MonoBehaviour
    {
        public string stepsRecordsSamplePath = "Assets/Resources/Data/StepsRecordsSample.json";
        public string sleepRecordsSamplePath = "Assets/Resources/Data/SleepSessionRecordsSample.json";
        public string exerciseRecordsSamplePath = "Assets/Resources/Data/ExerciseSessionRecordsSample.json";
        
        public static UserMetricsHandler Instance { get; private set; }
        
        /// <summary>
        /// Contains the user's steps count records. Can be null, so either null check or use the event instead.
        /// </summary>
        public IReadOnlyCollection<StepsRecord> StepsRecords { get; private set; }
        
        /// <summary>
        /// Contains the user's sleep session records. Can be null, so either null check or use the event instead.
        /// </summary>
        public IReadOnlyCollection<SleepSessionRecord> SleepSessionRecords { get; private set; }
        
        /// <summary>
        /// Contains the user's exercise session records. Can be null, so either null check or use the event instead.
        /// </summary>
        public IReadOnlyCollection<ExerciseSessionRecord> ExerciseSessionRecords { get; private set; } 
        
        public IReadOnlyCollection<HeartRateVariabilityRmssdRecord> HeartRateVariabilityRmssdRecords { get; private set; }
        public IReadOnlyCollection<ActiveCaloriesBurnedRecord> ActiveCaloriesBurnedRecords { get; private set; }
        public IReadOnlyCollection<Vo2MaxRecord> Vo2MaxRecords { get; private set; }
        
        public long TotalScreenTime { get; private set; }

        public event Action<IReadOnlyCollection<StepsRecord>> OnStepsRecordsUpdated;
        public event Action<IReadOnlyCollection<SleepSessionRecord>> OnSleepSessionRecordsUpdated;
        public event Action<IReadOnlyCollection<ExerciseSessionRecord>> OnExerciseSessionRecordsUpdated;
        public event Action<IReadOnlyCollection<HeartRateVariabilityRmssdRecord>> OnHeartRateVariabilityRecordsUpdated;
        public event Action<IReadOnlyCollection<ActiveCaloriesBurnedRecord>> OnActiveCaloriesBurnedRecordsUpdated;
        public event Action<IReadOnlyCollection<Vo2MaxRecord>> OnVo2MaxRecordsUpdated;
        
        public event Action<long> OnTotalScreenTimeUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log($"{nameof(UserMetricsHandler)} instance created");
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android) return;
            SetMockData();
        }

        public void SetData<T>(UserMetricsType userMetricsType, T data)
        {
            switch (userMetricsType)
            {
                case UserMetricsType.StepsRecords:
                    if (data is IReadOnlyCollection<StepsRecord> stepsRecords)
                    {
                        StepsRecords = stepsRecords;
                        Debug.Log("Steps records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, StepsRecords);
                        OnStepsRecordsUpdated?.Invoke(StepsRecords);
                    }
                    break;
                case UserMetricsType.SleepSessionRecords:
                    if (data is IReadOnlyCollection<SleepSessionRecord> sleepRecords)
                    {
                        SleepSessionRecords = sleepRecords;
                        Debug.Log("Sleep session records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, SleepSessionRecords);
                        OnSleepSessionRecordsUpdated?.Invoke(SleepSessionRecords);
                    }
                    break;
                case UserMetricsType.ExerciseSessionRecords:
                    if (data is IReadOnlyCollection<ExerciseSessionRecord> exerciseRecords)
                    {
                        ExerciseSessionRecords = exerciseRecords;
                        Debug.Log("Exercise records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, ExerciseSessionRecords);
                        OnExerciseSessionRecordsUpdated?.Invoke(ExerciseSessionRecords);
                    }
                    break;
                case UserMetricsType.ActiveCaloriesBurned:
                    if (data is IReadOnlyCollection<ActiveCaloriesBurnedRecord> caloriesRecords)
                    {
                        ActiveCaloriesBurnedRecords = caloriesRecords;
                        Debug.Log("Active calories burned records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, ActiveCaloriesBurnedRecords);
                        OnActiveCaloriesBurnedRecordsUpdated?.Invoke(caloriesRecords);
                    }
                    break;
                case UserMetricsType.HeartRateVariabilityRmssd:
                    if (data is IReadOnlyCollection<HeartRateVariabilityRmssdRecord> heartrateRecords)
                    {
                        HeartRateVariabilityRmssdRecords = heartrateRecords;
                        Debug.Log("Heartrate variability records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, HeartRateVariabilityRmssdRecords);
                        OnHeartRateVariabilityRecordsUpdated?.Invoke(heartrateRecords);
                    }
                    break;
                case UserMetricsType.Vo2Max:
                    if (data is IReadOnlyCollection<Vo2MaxRecord> vo2MaxRecords)
                    {
                        Vo2MaxRecords = vo2MaxRecords;
                        Debug.Log("Vo2 Max records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, Vo2MaxRecords);
                        OnVo2MaxRecordsUpdated?.Invoke(vo2MaxRecords);
                    }
                    break;
                case UserMetricsType.TotalScreenTime:
                    if (data is long totalScreenTime)
                    {
                        TotalScreenTime = totalScreenTime;
                        Debug.Log("Total screen time has been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, TotalScreenTime);
                        OnTotalScreenTimeUpdated?.Invoke(TotalScreenTime);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userMetricsType), userMetricsType, null);
            }
        }
        
        private void SetMockData()
        {
            if (File.Exists(stepsRecordsSamplePath))
            {
                SetData(UserMetricsType.StepsRecords, JsonConvert.DeserializeObject<IReadOnlyCollection<StepsRecord>>(File.ReadAllText(stepsRecordsSamplePath)));
            }
            else
            {
                Debug.LogWarning($"{nameof(UserMetricsHandler)} steps records sample file not found");
            }

            if (File.Exists(sleepRecordsSamplePath))
            {
                SetData(UserMetricsType.SleepSessionRecords, JsonConvert.DeserializeObject<IReadOnlyCollection<SleepSessionRecord>>(File.ReadAllText(sleepRecordsSamplePath)));
            }
            else
            {
                Debug.LogWarning($"{nameof(UserMetricsHandler)} sleep records sample file not found");
            }

            if (File.Exists(exerciseRecordsSamplePath))
            {
                SetData(UserMetricsType.ExerciseSessionRecords, JsonConvert.DeserializeObject<IReadOnlyCollection<ExerciseSessionRecord>>(File.ReadAllText(exerciseRecordsSamplePath)));
            }
            else
            {
                Debug.LogWarning($"{nameof(UserMetricsHandler)} exercise records sample file not found");
            }
            
            SetData(UserMetricsType.TotalScreenTime, (long)TimeSpan.FromHours(2).TotalMilliseconds);
            Debug.Log("Mock data has been set!");
        }
    }
}