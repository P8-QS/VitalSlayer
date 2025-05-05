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
        public const string HeartRateVariabilityRmssd = "HEART_RATE_VARIABILITY_RMSSD";
        public const string ActiveCaloriesBurned = "ACTIVE_CALORIES_BURNED";
        public const string TotalCaloriesBurned = "TOTAL_CALORIES_BURNED";
        public const string Vo2Max = "VO2_MAX";
    }

    public static class RequiredPermissions
    {
        public const string StepsRead = "android.permission.health.READ_STEPS";
        public const string SleepRead = "android.permission.health.READ_SLEEP";
        public const string ExerciseRead = "android.permission.health.READ_EXERCISE";
        public const string HeartRateVariabilityRead = "android.permission.health.READ_HEART_RATE_VARIABILITY";
        public const string ActiveCaloriesBurnedRead = "android.permission.health.READ_ACTIVE_CALORIES_BURNED";
        public const string TotalCaloriesBurnedRead = "android.permission.health.READ_TOTAL_CALORIES_BURNED";
        public const string Vo2MaxRead = "android.permission.health.READ_VO2_MAX";

        public static readonly string[] All =
        {
            StepsRead,
            SleepRead,
            ExerciseRead,
            HeartRateVariabilityRead,
            ActiveCaloriesBurnedRead,
            TotalCaloriesBurnedRead,
            Vo2MaxRead
        };
    }

    public class HealthConnectManager : MonoBehaviour
    {
        private AndroidJavaObject _healthConnectPlugin;
        private AndroidJavaObject _endLdt;
        private AndroidJavaObject _startLdt;

        public static HealthConnectManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.LogWarning("Not running on Android. Skipping Health Connect API initialization");
                return;
            }

            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-1);

            var ldt = new AndroidJavaClass("java.time.LocalDateTime");
            _endLdt = ldt.CallStatic<AndroidJavaObject>("of", endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
            _startLdt = ldt.CallStatic<AndroidJavaObject>("of", startDate.Year, startDate.Month, startDate.Day, 0, 0,
                0);

            InitializeHealthConnectClient();
        }

        private void InitializeHealthConnectClient()
        {
            var unityActivity = GetContext();

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

        #region Health Connect plugin callbacks

        private void OnHealthConnectUnavailable(string response)
        {
            Debug.Log("Received Health Connect unavailable response from HealthConnectPlugin");
            RedirectToPlayStore();
        }

        private void OnHealthConnectUpdateRequired(string response)
        {
            Debug.Log("Received Health Connect update required response from HealthConnectPlugin");
            RedirectToPlayStore(true);
        }

        private void OnHealthConnectAvailable(string response)
        {
            Debug.Log("Received Health Connect available response from HealthConnectPlugin");

            var context = GetContext();
            _healthConnectPlugin.Call("RequestPermissions", context);
            // context.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            // {
            // }));

            // using var pluginClass = new AndroidJavaClass("org.p8qs.healthconnectplugin.PermissionsTest");
            // pluginClass.CallStatic("showToast", context, "THIS IS A TEST");
            
            // var hasPerms = HasAllRequiredPermissions();
            //
            // if (!hasPerms)
            // {
            //     var callbacks = new PermissionCallbacks();
            //     callbacks.PermissionGranted += OnPermissionGranted;
            //     callbacks.PermissionDenied += OnPermissionDenied;
            //     callbacks.PermissionRequestDismissed += OnPermissionRequestDismissed;
            //     Permission.RequestUserPermissions(RequiredPermissions.All, callbacks);
            //     return;
            // }
            //
            // // All required permissions are available from this point
            // GetUserSteps();
            // GetUserSleepSessions();
            // GetUserExerciseSessions();
            // GetUserHeartRateVariability();
            // GetUserActiveCaloriesBurned();
            // GetUserTotalCaloriesBurned();
            // GetUserVo2Max();
        }

        #endregion

        #region Permission methods

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
                case RequiredPermissions.HeartRateVariabilityRead:
                    GetUserHeartRateVariability();
                    break;
                case RequiredPermissions.ActiveCaloriesBurnedRead:
                    GetUserActiveCaloriesBurned();
                    break;
                case RequiredPermissions.TotalCaloriesBurnedRead:
                    GetUserTotalCaloriesBurned();
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

        #endregion

        #region Get user data methods

        private void GetUserSteps()
        {
            Debug.Log(
                $"Getting user steps from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.Steps, gameObject.name,
                "OnStepsRecordsReceived");
        }

        private void OnStepsRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect steps data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<StepsRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.StepsRecords, records);
        }

        private void GetUserSleepSessions()
        {
            Debug.Log(
                $"Getting user sleep data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.SleepSession,
                gameObject.name, "OnSleepRecordsReceived");
        }

        private void OnSleepRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect sleep data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<SleepSessionRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.SleepSessionRecords, records);
        }

        private void GetUserExerciseSessions()
        {
            Debug.Log(
                $"Getting user exercise data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.ExerciseSession,
                gameObject.name, "OnExerciseRecordsReceived");
        }

        private void OnExerciseRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect exercise data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<ExerciseSessionRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.ExerciseSessionRecords, records);
        }

        private void GetUserActiveCaloriesBurned()
        {
            Debug.Log(
                $"Getting user active calories burned data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.ActiveCaloriesBurned,
                gameObject.name, "OnActiveCaloriesRecordsReceived");
        }

        private void OnActiveCaloriesRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect active calories burned data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<ActiveCaloriesBurnedRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.ActiveCaloriesBurned, records);
        }

        private void GetUserTotalCaloriesBurned()
        {
            Debug.Log(
                $"Getting user total calories burned data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.TotalCaloriesBurned,
                gameObject.name, "OnTotalCaloriesRecordsReceived");
        }

        private void OnTotalCaloriesRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect total calories burned data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<TotalCaloriesBurnedRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.TotalCaloriesBurned, records);
        }

        private void GetUserHeartRateVariability()
        {
            Debug.Log(
                $"Getting user heart rate variability data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.HeartRateVariabilityRmssd,
                gameObject.name, "OnHeartRateVariabilityRecordsReceived");
        }

        private void OnHeartRateVariabilityRecordsReceived(string response)
        {
            Debug.Log("Received Health Connect heart rate variability data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<HeartRateVariabilityRmssdRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.HeartRateVariabilityRmssd, records);
        }

        private void GetUserVo2Max()
        {
            Debug.Log(
                $"Getting user vo2 max data from: {_startLdt.Call<string>("toString")} to: {_endLdt.Call<string>("toString")}");

            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", _startLdt, _endLdt);

            _healthConnectPlugin.Call("ReadHealthRecords", timeRangeFilter, HealthRecordType.Vo2Max, gameObject.name,
                "OnVo2RecordsReceived");
        }

        private void OnVo2RecordsReceived(string response)
        {
            Debug.Log("Received Health Connect vo2 max data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<Vo2MaxRecord>>(response);

            if (records?.Count == 0) return;
            UserMetricsHandler.Instance.SetData(UserMetricsType.Vo2Max, records);
        }

        #endregion
        
        private static void RedirectToPlayStore(bool isUpdate = false)
        {
            var context = GetContext();
            
            try
            {
                context.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    var builder = new AndroidJavaObject(
                        "android.app.AlertDialog$Builder", context);
            
                    builder.Call<AndroidJavaObject>("setTitle", "Health Connect Required");
                    builder.Call<AndroidJavaObject>("setMessage", 
                        isUpdate 
                            ? "To use health tracking features, you need to update the Health Connect app."
                            : "To use health tracking features, you need to install the Health Connect app from Google Play Store.");
            
                    builder.Call<AndroidJavaObject>("setPositiveButton",
                        isUpdate ? "Update Now" : "Install Now", 
                        new DialogInterfaceOnClickListener(_ => {
                            OpenPlayStore(context);
                        }));
            
                    builder.Call<AndroidJavaObject>("setNegativeButton", "Not Now", 
                        new DialogInterfaceOnClickListener(_ => {
                            Debug.Log("User declined to install Health Connect");
                        }));
            
                    var dialog = builder.Call<AndroidJavaObject>("create");
                    dialog.Call("show");
                }));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to show dialog: " + e.Message);
                OpenPlayStore(context);
            }
        }
        
        private static void OpenPlayStore(AndroidJavaObject context)
        {
            Debug.Log("Redirecting user to Google Play Store to install Health Connect");
    
            try
            {
                const string packageName = "com.google.android.apps.healthdata";
        
                var intentClass = new AndroidJavaClass("android.content.Intent");
                var intent = new AndroidJavaObject("android.content.Intent");
        
                intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        
                var uriClass = new AndroidJavaClass("android.net.Uri");
                var uri = uriClass.CallStatic<AndroidJavaObject>("parse", 
                    "market://details?id=" + packageName);
        
                intent.Call<AndroidJavaObject>("setData", uri);
                context.Call("startActivity", intent);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to open Play Store app: " + e.Message);
                // OpenPlayStoreInBrowser();
            }
        }
        
        private static AndroidJavaObject GetContext()
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    internal class DialogInterfaceOnClickListener : AndroidJavaProxy
    {
        private Action<AndroidJavaObject> _onClick;
    
        public DialogInterfaceOnClickListener(Action<AndroidJavaObject> onClick) 
            : base("android.content.DialogInterface$OnClickListener")
        {
            _onClick = onClick;
        }
    
        public void onClick(AndroidJavaObject dialog, int which)
        {
            _onClick?.Invoke(dialog);
        }
    }
}