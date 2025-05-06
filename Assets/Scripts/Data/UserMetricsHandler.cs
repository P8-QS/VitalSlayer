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
        ActiveCaloriesBurnedRecords,
        ExerciseSessionRecords,
        HeartRateVariabilityRmssdRecords,
        StepsRecords,
        SleepSessionRecords,
        TotalCaloriesBurnedRecords,
        TotalScreenTime,
        Vo2MaxRecords,
    }
    
    public class UserMetricsHandler : MonoBehaviour
    {
        private const string StepsRecordsSamplePath = "Assets/Resources/Data/StepsRecordsSample.json";
        private const string SleepRecordsSamplePath = "Assets/Resources/Data/SleepSessionRecordsSample.json";
        private const string ExerciseRecordsSamplePath = "Assets/Resources/Data/ExerciseSessionRecordsSample.json";
        private const string AcbRecordsSamplePath = "Assets/Resources/Data/ActiveCaloriesBurnedRecordsSample.json";
        private const string TcbRecordsSamplePath = "Assets/Resources/Data/TotalCaloriesBurnedRecordsSample.json";
        private const string HrvRecordsSamplePath = "Assets/Resources/Data/HeartRateVariabilityRmssdRecordsSample.json";
        private const string Vo2MaxRecordsSamplePath = "Assets/Resources/Data/Vo2MaxRecordsSample.json";
        
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
        public IReadOnlyCollection<TotalCaloriesBurnedRecord> TotalCaloriesBurnedRecords { get; private set; }
        public IReadOnlyCollection<Vo2MaxRecord> Vo2MaxRecords { get; private set; }
        
        
        public long TotalScreenTime { get; private set; }

        public event Action<IReadOnlyCollection<StepsRecord>> OnStepsRecordsUpdated;
        public event Action<IReadOnlyCollection<SleepSessionRecord>> OnSleepSessionRecordsUpdated;
        public event Action<IReadOnlyCollection<ExerciseSessionRecord>> OnExerciseSessionRecordsUpdated;
        public event Action<IReadOnlyCollection<HeartRateVariabilityRmssdRecord>> OnHeartRateVariabilityRecordsUpdated;
        public event Action<IReadOnlyCollection<ActiveCaloriesBurnedRecord>> OnActiveCaloriesBurnedRecordsUpdated;
        public event Action<IReadOnlyCollection<TotalCaloriesBurnedRecord>> OnTotalCaloriesBurnedRecordsUpdated; 
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
                case UserMetricsType.ActiveCaloriesBurnedRecords:
                    if (data is IReadOnlyCollection<ActiveCaloriesBurnedRecord> acbRecords)
                    {
                        ActiveCaloriesBurnedRecords = acbRecords;
                        Debug.Log("Active calories burned records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, ActiveCaloriesBurnedRecords);
                        OnActiveCaloriesBurnedRecordsUpdated?.Invoke(acbRecords);
                    }
                    break;
                case UserMetricsType.TotalCaloriesBurnedRecords:
                    if (data is IReadOnlyCollection<TotalCaloriesBurnedRecord> tcbRecords)
                    {
                        TotalCaloriesBurnedRecords = tcbRecords;
                        Debug.Log("Total calories burned records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, TotalCaloriesBurnedRecords);
                        OnTotalCaloriesBurnedRecordsUpdated?.Invoke(tcbRecords);
                    }
                    break;
                case UserMetricsType.HeartRateVariabilityRmssdRecords:
                    if (data is IReadOnlyCollection<HeartRateVariabilityRmssdRecord> heartrateRecords)
                    {
                        HeartRateVariabilityRmssdRecords = heartrateRecords;
                        Debug.Log("Heart rate variability records have been updated");
                        LoggingManager.Instance.LogMetric(userMetricsType, HeartRateVariabilityRmssdRecords);
                        OnHeartRateVariabilityRecordsUpdated?.Invoke(heartrateRecords);
                    }
                    break;
                case UserMetricsType.Vo2MaxRecords:
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

        public void EmitEvents()
        {
            OnStepsRecordsUpdated?.Invoke(StepsRecords);
            OnSleepSessionRecordsUpdated?.Invoke(SleepSessionRecords);
            OnExerciseSessionRecordsUpdated?.Invoke(ExerciseSessionRecords);
            OnActiveCaloriesBurnedRecordsUpdated?.Invoke(ActiveCaloriesBurnedRecords);
            OnTotalCaloriesBurnedRecordsUpdated?.Invoke(TotalCaloriesBurnedRecords);
            OnHeartRateVariabilityRecordsUpdated?.Invoke(HeartRateVariabilityRmssdRecords);
            OnVo2MaxRecordsUpdated?.Invoke(Vo2MaxRecords);
            OnTotalScreenTimeUpdated?.Invoke(TotalScreenTime);
        }
        
        private void SetMockData()
        {
            TrySetDataFromFile<StepsRecord>(UserMetricsType.StepsRecords, StepsRecordsSamplePath);
            TrySetDataFromFile<SleepSessionRecord>(UserMetricsType.SleepSessionRecords, SleepRecordsSamplePath);
            TrySetDataFromFile<ExerciseSessionRecord>(UserMetricsType.ExerciseSessionRecords, ExerciseRecordsSamplePath);
            TrySetDataFromFile<ActiveCaloriesBurnedRecord>(UserMetricsType.ActiveCaloriesBurnedRecords, AcbRecordsSamplePath);
            TrySetDataFromFile<TotalCaloriesBurnedRecord>(UserMetricsType.TotalCaloriesBurnedRecords, TcbRecordsSamplePath);
            TrySetDataFromFile<HeartRateVariabilityRmssdRecord>(UserMetricsType.HeartRateVariabilityRmssdRecords, HrvRecordsSamplePath);
            TrySetDataFromFile<Vo2MaxRecord>(UserMetricsType.Vo2MaxRecords, Vo2MaxRecordsSamplePath);
            
            SetData(UserMetricsType.TotalScreenTime, (long)TimeSpan.FromHours(2).TotalMilliseconds);
            Debug.Log("Mock data has been set!");
        }
        
        private void TrySetDataFromFile<T>(UserMetricsType type, string path)
        {
            if (File.Exists(path))
            {
                var data = JsonConvert.DeserializeObject<IReadOnlyCollection<T>>(File.ReadAllText(path));
                SetData(type, data);
            }
            else
            {
                Debug.LogWarning($"{nameof(UserMetricsHandler)} {type} sample file not found");
            }
        }
    }
}