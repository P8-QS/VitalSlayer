using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
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
        public static UserMetricsHandler Instance { get; private set; }
        
        /// <summary>
        /// Contains the user's steps count records.
        /// </summary>
        public IReadOnlyCollection<StepsRecord> StepsRecords { get; private set; }
        
        /// <summary>
        /// Contains the user's sleep session records.
        /// </summary>
        public IReadOnlyCollection<SleepSessionRecord> SleepSessionRecords { get; private set; }
        
        public long TotalScreenTime { get; private set; }

        public event Action<IReadOnlyCollection<StepsRecord>> OnStepsRecordsUpdated;
        public event Action<IReadOnlyCollection<SleepSessionRecord>> OnSleepSessionRecordsUpdated;
        
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

        public void SetData<T>(UserMetricsType userMetricsType, T data)
        {
            switch (userMetricsType)
            {
                case UserMetricsType.StepsRecords:
                    if (data is IReadOnlyCollection<StepsRecord> stepsRecords)
                    {
                        StepsRecords = stepsRecords;
                        Debug.Log("Steps records have been updated");
                        OnStepsRecordsUpdated?.Invoke(StepsRecords);
                    }
                    break;
                case UserMetricsType.SleepSessionRecords:
                    if (data is IReadOnlyCollection<SleepSessionRecord> sleepRecords)
                    {
                        SleepSessionRecords = sleepRecords;
                        Debug.Log("Sleep session records have been updated");
                        OnSleepSessionRecordsUpdated?.Invoke(SleepSessionRecords);
                    }
                    break;
                case UserMetricsType.TotalScreenTime:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userMetricsType), userMetricsType, null);
            }
        }
    }
}