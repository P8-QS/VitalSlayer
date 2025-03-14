using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct ScreenTime {
    public double screenTime;
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
    private void Start()
    {
        StartCoroutine(CheckAndRequestPermission());
    }

    private IEnumerator CheckAndRequestPermission() {
        while(!CheckPermission()) {
            Debug.Log("UsageStats Permission Denied");

            RequestPermission();
            yield return new WaitForSeconds(1);
        }

        Debug.Log("UsageStats Permission Granted");
        OnPermissionGranted();
    }

    private void OnPermissionGranted() {
        DateTimeOffset startTime = DateTimeOffset.UtcNow.Date.AddDays(-1);
        DateTimeOffset endTime = startTime.AddDays(1);
        try {
            List<ScreenTime> screenTimeList = GetScreenTime(startTime.ToUnixTimeMilliseconds(), endTime.ToUnixTimeMilliseconds(), TimeUnit.Seconds);

            double totalScreenTime = 0.0;
            foreach (ScreenTime screenTime in screenTimeList) {
                Debug.Log($"Package name: {screenTime.packageName}, Screen time: {screenTime.screenTime}");
                totalScreenTime += screenTime.screenTime;
            }
            Debug.Log($"Total screen time: {totalScreenTime}");
        }
        catch (Exception e) {
            Debug.Log(e);
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

    private void RequestPermission()
    {
        using (AndroidJavaObject context = GetContext())
        {
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.USAGE_ACCESS_SETTINGS"))
            {
                context.Call("startActivity", intent);
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

    private double ConvertTime(long milliseconds, TimeUnit timeUnit) {
        double time = 0.0;

        switch (timeUnit) {
            case TimeUnit.Milliseconds: {
                time = milliseconds;
                break;
            }
            case TimeUnit.Seconds: {
                time = milliseconds / 1000.0;
                break;
            }
            case TimeUnit.Minutes: {
                time = milliseconds / (1000.0 * 60.0);
                break;
            }
            case TimeUnit.Hours: {
                time = milliseconds / (1000.0 * 60.0 * 60.0);
                break;
            }
            case TimeUnit.Days: {
                time = milliseconds / (1000.0 * 60.0 * 60.0 * 24.0);
                break;
            }
        }

        return time;
    }

    private List<ScreenTime> GetScreenTime(long startTime, long endTime, TimeUnit timeUnit)
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
                            double packageScreenTime = ConvertTime(packageScreenTimeMilliseconds, timeUnit); 
                            ScreenTime screenTime = new ScreenTime{ packageName = packageName, screenTime = packageScreenTime};
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
}
