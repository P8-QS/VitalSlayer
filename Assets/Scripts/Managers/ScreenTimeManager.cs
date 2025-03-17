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
    private void Awake()
    {   
        StartCoroutine(CheckAndRequestPermission());
    }

    private IEnumerator CheckAndRequestPermission() {
        while(!CheckPermission()) {
            RequestPermission();
            yield return new WaitForSeconds(1);
        }
        OnPermissionGranted();
    }

    private void OnPermissionGranted() {
        DateTimeOffset startTime = DateTimeOffset.UtcNow.Date.AddDays(-1);
        DateTimeOffset endTime = startTime.AddDays(1);
        long totalScreenTime = 0;
        try {
            List<ScreenTime> screenTimeList = GetScreenTime(startTime.ToUnixTimeMilliseconds(), endTime.ToUnixTimeMilliseconds());

            foreach (ScreenTime screenTime in screenTimeList) {
                totalScreenTime += screenTime.screenTime;
            }
            UserMetricsHandler.Instance.SetData(UserMetricsType.TotalScreenTime, totalScreenTime);
        }
        catch (Exception e) {
            Debug.LogError(e);
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
}
