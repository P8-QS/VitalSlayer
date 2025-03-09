using System;
using UnityEngine;
using UnityEngine.Android;

namespace Managers
{
    public class HealthConnectManager : MonoBehaviour
    {
        private AndroidJavaObject _healthConnectClient;
        private AndroidJavaObject _healthConnectPlugin;
        
        private const string ProviderPackageName = "com.google.android.apps.healthdata";
    
        private void Start()
        {
            TestPlugin();
            // CheckHealthConnectAvailability();
        }

        private void TestPlugin()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.Log("Not running on Android");
                return;
            }
            
            var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                _healthConnectPlugin = new AndroidJavaObject("org.p8qs.healthconnectplugin.UnityPlugin", unityActivity);
                
                Debug.Log("Checking for health connect permissions");
                var hasPerms = _healthConnectPlugin.Call<int>("HasPermissions");
                
                if (hasPerms == 1)
                {
                    Debug.LogWarning("Has Permissions");
                    _healthConnectPlugin.Call("ReadStepsDay");
                }
                else
                {
                    Debug.LogWarning("Permission denied");
                    _healthConnectPlugin.Call("ReadStepsDay");

                    // RequestHealthConnectPermissions(unityActivity);
                }
            }));
            
            
        }

        private void CheckHealthConnectAvailability()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.Log("Not running on Android, skipping Health Connect check.");
                return;
            }
            
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var healthConnectClientClass = new AndroidJavaClass("androidx.health.connect.client.HealthConnectClient");
            var availabilityStatus = healthConnectClientClass.CallStatic<int>("getSdkStatus", activity, ProviderPackageName);

            if (availabilityStatus == healthConnectClientClass.GetStatic<int>("SDK_UNAVAILABLE")) 
            {
                Debug.Log("Health Connect SDK is unavailable.");
                return;
            }

            if (availabilityStatus == healthConnectClientClass.GetStatic<int>("SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED"))
            {
                Debug.Log("Health Connect requires an update.");
                RedirectToPlayStore();
            }

            Debug.Log("Health Connect is available.");
            _healthConnectClient = healthConnectClientClass.CallStatic<AndroidJavaObject>("getOrCreate", activity);

            RequestHealthConnectPermissions(activity);
        }

        private void RequestHealthConnectPermissions(AndroidJavaObject activity)
        {
            const string readStepsPermission = "android.permission.health.READ_STEPS";

            if (Permission.HasUserAuthorizedPermission(readStepsPermission))
            {
                Debug.Log("Reading steps permission has been granted.");
                OnStepsPermissionGranted(readStepsPermission);
            }
            else
            {
                Debug.Log("Reading steps permission is not granted. Requesting permission.");
                
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += OnStepsPermissionGranted;
                Permission.RequestUserPermission(readStepsPermission, callbacks);
            }
        }

        private void OnStepsPermissionGranted(string permissionName)
        {
            Debug.Log($"OnStepsPermissionGranted: {permissionName}");
            _healthConnectPlugin.Call("ReadStepsDay");

            
            // var stepsCount = GetDayStepCount();
            // Debug.Log($"Steps count: {stepsCount}");
        }

        private int GetDayStepCount()
        {
            var ldt = new AndroidJavaClass("java.time.LocalDateTime");
            var now = ldt.CallStatic<AndroidJavaObject>("now");
            var yesterday = now.Call<AndroidJavaObject>("minusDays", (long)1);
            
            var stepsRecordClass = new AndroidJavaClass("androidx.health.connect.client.records.StepsRecord");
            
            var timeRangeFilterClass = new AndroidJavaClass("androidx.health.connect.client.time.TimeRangeFilter");
            var timeRangeFilter = timeRangeFilterClass.CallStatic<AndroidJavaObject>("between", yesterday, now);
            
            var dataOrigin = new AndroidJavaObject("androidx.health.connect.client.records.metadata.DataOrigin", "QSCrawler");
            
            Debug.Log("Created dataOrigin");
            
            var readRequestClass = new AndroidJavaClass("androidx.health.connect.client.request.ReadRecordsRequest");
            Debug.Log("Created request class");
            
            // TODO: FIIIIIIIIIIIIIIIIIIIIIIX
            object[] parameters = {stepsRecordClass, timeRangeFilter, dataOrigin, true, 100, "1"};

            Debug.Log($"stepsrecordclass print: {stepsRecordClass}");
            

            var readRequest = readRequestClass.Call<AndroidJavaObject>(
                "getConstructor",
                stepsRecordClass
                // parameters
            );
            // var readRequest = new AndroidJavaObject("androidx.health.connect.client.request.ReadRecordsRequest", parameters);
            Debug.Log("Created request class");

            
            // Debug.Log($"Read records request: {readRequest}");

            
            return 0;
        }

        private void RedirectToPlayStore()
        {
            throw new System.NotImplementedException();
        }
    }
}
