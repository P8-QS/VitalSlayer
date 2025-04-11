using System;
using System.Collections.Generic;
using System.IO;
using Data;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class LogEntry
{
    public string timestamp;
    public string eventName;
    public Dictionary<string, object> data;

    public LogEntry(string eventName, Dictionary<string, object> data)
    {
        this.eventName = eventName;
        this.timestamp = DateTime.UtcNow.ToString("o");
        this.data = data;
    }
}

public class LoggingManager : MonoBehaviour
{
    private string logFilePath;
    private DateTime sessionStart;
    private string sessionId;

    public static LoggingManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
           Destroy(gameObject);
           return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var timestamp = DateTime.UtcNow;
        sessionStart = timestamp;
        sessionId = PlayerPrefs.GetString("unity.player_sessionid");
        string dir = Path.Combine(Application.persistentDataPath, "logs");
        Directory.CreateDirectory(dir);
        logFilePath = Path.Combine(dir, $"session_{sessionId}_{timestamp.ToString("yyyy-MM-dd_HH-mm-ss")}.jsonl");

        LogDeviceInfo();
    }

    public void LogEvent(string eventName, Dictionary<string, object> data)
    {
        var entry = new LogEntry(eventName, data);
        string json = JsonConvert.SerializeObject(entry);
        File.AppendAllText(logFilePath, json + "\n");
        Debug.Log($"[LOGGED] {json}");
    }

    public void LogMetric<T>(UserMetricsType type, T data)
    {
        var log = new Dictionary<string, object>{
            {type.ToString(), data}
        };

        LogEvent("metric", log);
    }

    private void LogDeviceInfo() 
    {
        var log = new Dictionary<string, object>{
            {"deviceModel", SystemInfo.deviceModel},
            {"operatingSystem", SystemInfo.operatingSystem},
            {"screenWidth", Screen.width},
            {"screenHeight", Screen.height},
            {"appVersion", Application.version},
            {"platform", Application.platform.ToString()}
        };

        LogEvent("deviceInfo", log);
    }

    void OnApplicationQuit()
    {
        var sessionEnd = DateTime.UtcNow;
        var sessionDuration = (sessionEnd - sessionStart).TotalSeconds;

        string user_id;
        if (PlayerPrefs.HasKey("user_id"))
        {
            user_id = PlayerPrefs.GetString("user_id");
        }
        else
        {
            user_id = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("user_id", user_id);
            PlayerPrefs.Save();
        }

        var log = new Dictionary<string, object>{
            {"sessionId", sessionId},
            {"sessionStartTime", sessionStart},
            {"sessionEndTime", sessionEnd},
            {"sessionDurationSeconds", sessionDuration},
            {"userId", user_id}
        };

        LogEvent("sessionInfo", log);
    }

    public string GetLogFilePath() {
        return logFilePath;
    }
}