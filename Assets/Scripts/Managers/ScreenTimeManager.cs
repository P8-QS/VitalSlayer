using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;

public struct ScreenTime {
    public long screenTime;
    public string packageName;
}

public enum TimeUnit {
    Milliseconds,
    Seconds,
    Minutes,
    Hours,
    Days
}

public class ScreenTimeManager : MonoBehaviour
{
    private bool openedUsageAccessSettings = false;
    private void Awake()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.LogWarning("Not running on Android. Skipping Screen Time Manager initialization");
            return;    
        }
        
        CheckAndRequestPermission();
    }

    private void CheckAndRequestPermission() {
        if (CheckPermission())
        {
            OnPermissionGranted();
        }
        else
        {
            ShowNativeDialog();
        }
    }

    private void OnPermissionGranted() {
        DateTimeOffset startTime = DateTimeOffset.UtcNow.Date.AddDays(-1);
        DateTimeOffset endTime = startTime.AddDays(1);
        long totalScreenTime = 0;
        try {
            List<ScreenTime> screenTimeList = GetScreenTime(startTime.ToUnixTimeMilliseconds(), endTime.ToUnixTimeMilliseconds());

            if (screenTimeList == null) {
                Debug.LogWarning("Screen time is null");
                return;
            }

            foreach (ScreenTime screenTime in screenTimeList) {
                totalScreenTime += screenTime.screenTime;
            }
            UserMetricsHandler.Instance.SetData(UserMetricsType.TotalScreenTime, totalScreenTime);
        }
        catch (Exception e) {
            Debug.LogError(e);
        } 
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && openedUsageAccessSettings)
        {
            openedUsageAccessSettings = false;

            if (CheckPermission())
            {
                OnPermissionGranted();
            }
        }
    }

    private bool CheckPermission()
    {
        using (AndroidJavaObject context = GetContext())
        {
            using (AndroidJavaClass appOpsManagerClass = new AndroidJavaClass("android.app.AppOpsManager"))
            {
                int mode;
                using (AndroidJavaObject appOpsManager = context.Call<AndroidJavaObject>("getSystemService", "appops"))
                {
                    string packageName = context.Call<string>("getPackageName");
                    int uid = context.Call<AndroidJavaObject>("getApplicationInfo").Get<int>("uid");
                    
                    mode = appOpsManager.Call<int>("checkOpNoThrow", "android:get_usage_stats", uid, packageName);
                }

                return mode == 0; 
            }
        }
    }

    private void OpenUsageAccessSettings()
    {
        using (AndroidJavaObject context = GetContext())
        {
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.USAGE_ACCESS_SETTINGS"))
            {
                context.Call("startActivity", intent);
                openedUsageAccessSettings = true;
            }
        }
    }

    private AndroidJavaObject GetContext()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
    public void ShowNativeDialog()
    {
        AndroidJavaObject context = GetContext();
        context.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            using (AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", context))
            {
                builder.Call<AndroidJavaObject>("setTitle", "Permission Needed");
                var message = "To work optimally this app requires access to your Usage Data.\n\n" +
                              "Please grant \"Usage Access\" permission in settings by finding this app in the list and enabling access.";
                builder.Call<AndroidJavaObject>("setMessage", message);
                builder.Call<AndroidJavaObject>("setCancelable", false);
                builder.Call<AndroidJavaObject>("setPositiveButton", "Open Settings", new DialogOnClickListener(() =>
                {
                    OpenUsageAccessSettings();
                }));
                builder.Call<AndroidJavaObject>("setNegativeButton", "Maybe Later", null);
                builder.Call<AndroidJavaObject>("show");
            }
        }));
    }

    private List<ScreenTime> GetScreenTime(long startTime, long endTime)
    {
        using (AndroidJavaObject context = GetContext())
        {
            using (AndroidJavaClass usageStatsManagerClass = new AndroidJavaClass("android.app.usage.UsageStatsManager"))
            {
                AndroidJavaObject usageStatsManager = context.Call<AndroidJavaObject>("getSystemService", "usagestats");
                using (AndroidJavaObject stats = usageStatsManager.Call<AndroidJavaObject>("queryUsageStats", 4, startTime, endTime))
                {
                    if (stats != null)
                    {

                        int count = stats.Call<int>("size");
                        List<ScreenTime> screenTimeList = new List<ScreenTime>();
                        for (int i = 0; i < count; i++)
                        {
                            AndroidJavaObject usageStats = stats.Call<AndroidJavaObject>("get", i);
                            string packageName = usageStats.Call<string>("getPackageName");
                            long packageScreenTimeMilliseconds = usageStats.Call<long>("getTotalTimeInForeground");
                            ScreenTime screenTime = new ScreenTime{ packageName = packageName, screenTime = packageScreenTimeMilliseconds};
                            screenTimeList.Add(screenTime);
                            
                        }
                        return screenTimeList;
                    }
                    else
                    {
                        throw new Exception($"No usage stats available. startTime: {startTime}, endTime: {endTime}");
                    }
                }
            }
        }
    }
    private class DialogOnClickListener : AndroidJavaProxy
    {
        private System.Action callback;

        public DialogOnClickListener(System.Action callback)
            : base("android.content.DialogInterface$OnClickListener")
        {
            this.callback = callback;
        }

        public void onClick(AndroidJavaObject dialog, int which)
        {
            callback?.Invoke();
        }
    }
}

