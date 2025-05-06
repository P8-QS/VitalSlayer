using System;
using System.Collections.Generic;
using Data;
using Data.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Managers
{
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
        }

        #endregion
        
        #region Get user data methods

        private void OnStepsRecordsReceived(string response) =>
            OnDataRecordsReceived<StepsRecord>(response, UserMetricsType.StepsRecords);

        private void OnSleepRecordsReceived(string response) =>
            OnDataRecordsReceived<SleepSessionRecord>(response, UserMetricsType.SleepSessionRecords);

        private void OnExerciseRecordsReceived(string response) =>
            OnDataRecordsReceived<ExerciseSessionRecord>(response, UserMetricsType.ExerciseSessionRecords);

        private void OnActiveCaloriesRecordsReceived(string response) =>
            OnDataRecordsReceived<ActiveCaloriesBurnedRecord>(response, UserMetricsType.ActiveCaloriesBurnedRecords);

        private void OnTotalCaloriesRecordsReceived(string response) =>
            OnDataRecordsReceived<TotalCaloriesBurnedRecord>(response, UserMetricsType.TotalCaloriesBurnedRecords);

        private void OnHeartRateVariabilityRecordsReceived(string response) =>
            OnDataRecordsReceived<HeartRateVariabilityRmssdRecord>(response, UserMetricsType.HeartRateVariabilityRmssdRecords);

        private void OnVo2RecordsReceived(string response) =>
            OnDataRecordsReceived<Vo2MaxRecord>(response, UserMetricsType.Vo2MaxRecords);

        private static void OnDataRecordsReceived<T>(string response, UserMetricsType userMetricsType, bool isHistory = false)
        {
            Debug.Log($"Received Health Connect {Enum.GetName(typeof(UserMetricsType), userMetricsType)} data response from HealthConnectPlugin!");
            var records = JsonConvert.DeserializeObject<IReadOnlyCollection<T>>(response);
            
            if (records?.Count == 0) return;
            

            UserMetricsHandler.Instance.SetData(userMetricsType, records);
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