using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Android;

namespace Managers
{
    public static class HealthRecordType
    {
        public const string Steps = "STEPS";
        public const string SleepSession = "SLEEP_SESSION";
        public const string ExerciseSession = "EXERCISE_SESSION";
        public const string HeartRateVariability = "HEART_RATE_VARIABILITY";
        public const string ActiveCaloriesBurned = "ACTIVE_CALORIES_BURNED";
        public const string Vo2Max = "VO2_MAX";
    }

    public static class RequiredPermissions
    {
        public const string StepsRead = "android.permission.health.READ_STEPS";
        public const string SleepRead = "android.permission.health.READ_SLEEP";
        public const string ExerciseRead = "android.permission.health.READ_EXERCISE";
        public const string HeartRateVariabilityRead = "android.permission.health.READ_HEART_RATE_VARIABILITY";
        public const string ActiveCaloriesBurnedRead = "android.permission.health.READ_ACTIVE_CALORIES_BURNED";
        public const string Vo2MaxRead = "android.permission.health.READ_VO2_MAX";

        public static readonly string[] All = { StepsRead, SleepRead, ExerciseRead };
    }
    
    public class HealthConnectManager : MonoBehaviour
    {
        private AndroidJavaObject _healthConnectPlugin;
        private AndroidJavaObject _endLdt;
        private AndroidJavaObject _startLdt;
        
        private void Awake()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.LogWarning("Not running on Android. Skipping Health Connect API initialization");
                return;
            }
            
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-1);

            var ldt = new AndroidJavaClass("java.time.LocalDateTime");
            _endLdt = ldt.CallStatic<AndroidJavaObject>("of", endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
            _startLdt = ldt.CallStatic<AndroidJavaObject>("of", startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            
            InitializeHealthConnectClient();
        }

        private void InitializeHealthConnectClient()
        {
            var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                _healthConnectPlugin = new AndroidJavaObject("org.p8qs.healthconnectplugin.UnityPlugin", unityActivity);
                
                _healthConnectPlugin.Call(
                    "CheckHealthConnectAvailability",
                    gameObject.name,
                    "OnHealthConnectUnavailable",
                    "OnHealthConnectUpdateRequired",
                    "OnHealthConnectAvailable"
                );
            }));
        }

        private void OnHealthConnectUnavailable(string response)
        {
            Debug.Log("Received Health Connect unavailable response from HealthConnectPlugin");
            RedirectToPlayStore();
        }
        
        private void OnHealthConnectUpdateRequired(string response)
        {
            Debug.Log("Received Health Connect update required response from HealthConnectPlugin");
            // TODO: Implement logic for requiring user to update Health Connect app.
        }
        
        private void OnHealthConnectAvailable(string response)
        {
            Debug.Log("Received Health Connect available response from HealthConnectPlugin");
            var hasPerms = HasAllRequiredPermissions();
            
            if (!hasPerms)
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += OnPermissionGranted;
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionRequestDismissed += OnPermissionRequestDismissed;
                Permission.RequestUserPermissions(RequiredPermissions.All, callbacks);
                return;
            }
            
            // All required permissions are available from this point
            GetUserSteps();
            GetUserSleepSessions();
            GetUserExerciseSessions();
            GetUserVo2Max();
        }
        
        private static bool HasAllRequiredPermissions()
        {
            var authorizedPermissions = RequiredPermissions.All.Select(Permission.HasUserAuthorizedPermission);
            
            if (authorizedPermissions.Any(permission => permission == false))
            {
                Debug.Log("All Health Connect permissions have not been granted. Requesting from user");
                return false;    
            }

            Debug.Log("All Health Connect permissions have been granted.");
            return true;
        }

        private void OnPermissionGranted(string permissionName)
        {
            // This method is called for each permission that is granted by the user
            Debug.Log($"Granted permission: {permissionName}");

            switch (permissionName)
            {
                case RequiredPermissions.StepsRead:
                    GetUserSteps();
                    break;
                case RequiredPermissions.SleepRead:
                    GetUserSleepSessions();
                    break;
                case RequiredPermissions.ExerciseRead:
                    GetUserExerciseSessions();
                    break;
                case RequiredPermissions.Vo2MaxRead:
                    GetUserVo2Max();
                    break;
            }
        }
        
        private void OnPermissionDenied(string permissionName)
        {
            // This method is called for each permission that is granted by the user
            Debug.Log($"Denied permission: {permissionName}");
            // TODO: Implement permission denied logic
        }
        
        private void OnPermissionRequestDismissed(string permissionName)
        {
            // This method is called for each permission that is granted by the user
            Debug.Log($"Permission request dismissed: {permissionName}");
            // TODO: Implement permission request dismissed logic
        }
        
        private void GetUserSteps()
        {
            Debug.Log($"Getting user steps from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");
            
            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);
            
            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.Steps, gameObject.name, "OnStepsRecordsReceived");
        }

        private void OnStepsRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect steps data response from HealthConnectPlugin!");

            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<StepsRecord>>(response);
            UserMetricsHandler.Instance.SetData(UserMetricsType.StepsRecords, records);
        }
        
        private void GetUserSleepSessions()
        {
            Debug.Log($"Getting user sleep data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");
            
            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);
            
            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.SleepSession, gameObject.name, "OnSleepRecordsReceived");
        }

        private void OnSleepRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect sleep data response from HealthConnectPlugin!");
            
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<SleepSessionRecord>>(response);
            UserMetricsHandler.Instance.SetData(UserMetricsType.SleepSessionRecords, records);
        }
        
        private void GetUserExerciseSessions()
        {
            Debug.Log($"Getting user exercise data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");
            
            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);
            
            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.ExerciseSession, gameObject.name, "OnExerciseRecordsReceived");
        }
        
        private void OnExerciseRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect exercise data response from HealthConnectPlugin!");
            
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<ExerciseSessionRecord>>(response);
            UserMetricsHandler.Instance.SetData(UserMetricsType.ExerciseSessionRecords, records);
        }
        
        private void GetUserVo2Max()
        {
            Debug.Log($"Getting user vo2 max data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");
            
            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);
            
            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.SleepSession, gameObject.name, "OnVo2RecordsReceived");
        }

        private void OnVo2RecordsReceived(string response)
        {
            Debug.Log("Received Health Connect vo2 max data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<Vo2MaxRecord>>(response);
            UserMetricsHandler.Instance.SetData(UserMetricsType.Vo2Max, records);
        }
        
        private void RedirectToPlayStore()
        {
            // TODO: Implement logic for when user does not have Health Connect installed.
        }
    }
}
