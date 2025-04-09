using System;
using System.Collections.Generic;
using System.Data;
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
    private static LoggingManager _instance;
    private string logFilePath;
    private DateTime sessionStart;
    private string sessionId;

    public static LoggingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("LoggingManager");
                _instance = obj.AddComponent<LoggingManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    void Awake()
    {
        var timestamp = DateTime.UtcNow;
        sessionStart = timestamp;
        sessionId = PlayerPrefs.GetString("unity.player_sessionid");
        string dir = Path.Combine(Application.persistentDataPath, "logs");
        Directory.CreateDirectory(dir);
        logFilePath = Path.Combine(dir, $"session_{sessionId}_{timestamp.ToString("o")}.jsonl");

        LogDeviceInfo();
    }

    public void LogEvent(string eventName, Dictionary<string, object> data)
    {
        var entry = new LogEntry(eventName, data);
        string json = JsonConvert.SerializeObject(entry);
        File.AppendAllText(logFilePath, json + "\n");
        Debug.Log($"[LOGGED] {json}");
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

        var log = new Dictionary<string, object>{
            {"sessionId", sessionId},
            {"sessionStartTime", sessionStart},
            {"sessionEndTime", sessionEnd},
            {"sessionDurationSeconds", sessionDuration}
        };

        LogEvent("sessionInfo", log);
    }
}