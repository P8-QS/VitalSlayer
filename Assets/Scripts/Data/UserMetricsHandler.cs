using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public enum UserMetricsType
    {
        StepsRecords,
        SleepSessionRecords,
        TotalScreenTime
    }
    
    public class UserMetricsHandler : MonoBehaviour
    {
        public string stepsRecordsSamplePath = "Assets/Resources/Data/StepsRecordsSample.json";
        public string sleepRecordsSamplePath = "Assets/Resources/Data/SleepSessionRecordsSample.json";
        
        public static UserMetricsHandler Instance { get; private set; }
        
        /// <summary>
        /// Contains the user's steps count records. Can be null, so either null check or use the event instead.
        /// </summary>
        public IReadOnlyCollection<StepsRecord> StepsRecords { get; private set; }
        
        /// <summary>
        /// Contains the user's sleep session records. Can be null, so either null check or use the event instead.
        /// </summary>
        public IReadOnlyCollection<SleepSessionRecord> SleepSessionRecords { get; private set; }
        
        public long TotalScreenTime { get; private set; }

        public event Action<IReadOnlyCollection<StepsRecord>> OnStepsRecordsUpdated;
        public event Action<IReadOnlyCollection<SleepSessionRecord>> OnSleepSessionRecordsUpdated;
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
            
            SetData(UserMetricsType.TotalScreenTime, (long)TimeSpan.FromHours(2).TotalMilliseconds);
            Debug.Log("Mock data has been set!");
        }
    }
}