using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Newtonsoft.Json;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    public static LoggingManager Instance;
    private bool _isReady = false;
    private Queue _queuedEvents = new();

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
           Destroy(gameObject);
           return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("UnityServices Initialized!");
        AnalyticsService.Instance.StartDataCollection();

        _isReady = true;

        while (_queuedEvents.Count > 0)
        {
            var logEvent = (Unity.Services.Analytics.Event)_queuedEvents.Dequeue();
            AnalyticsService.Instance.RecordEvent(logEvent);
        }
    }

    private void LogEvent(Unity.Services.Analytics.Event eventLog)
    {
        if (_isReady)
        {
            AnalyticsService.Instance.RecordEvent(eventLog);
        }
        else
        {
            _queuedEvents.Enqueue(eventLog);
        }
    }

    public void LogMetric<T>(UserMetricsType type, T data)
    {
        object dataCopy = data;

        if (type is not UserMetricsType.TotalScreenTime) {
            dataCopy = JsonConvert.SerializeObject(data);
        }
        var metricSetEvent = new MetricSetEvent(type, dataCopy);

        LogEvent(metricSetEvent);
    }

    public void LogHistoricalMetric<T>(UserMetricsType type, T data)
    {
        object dataCopy = data;
        if (type is not UserMetricsType.TotalScreenTime)
        {
            dataCopy = JsonConvert.SerializeObject(data);
        }
        var historicMetricEvent = new HistoricMetricEvent(type, dataCopy);

        LogEvent(historicMetricEvent);
    }

    public void LogGameSummary(bool gameWon, int totalEnemies, int xpGained, int levelsGained, DateTime roundStartTime)
    {
        var roundEndTime = DateTime.UtcNow;
        var roundDuration = (roundEndTime - roundStartTime).ToString();

        var gameSummaryEvent = new GameRoundCompleted
        {
            GameWon = gameWon,
            TotalEnemiesKilled = totalEnemies,
            XpGained = xpGained,
            LevelsGained = levelsGained,
            RoundDuration = roundDuration
        };

        LogEvent(gameSummaryEvent);
    }

    public void LogEmail(string email)
    {
        var emailEvent = new EmailSetEvent { Email = email };
        LogEvent(emailEvent);
    }
}