using System;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
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

        UnityServices.Instance.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void LogMetric<T>(UserMetricsType type, T data)
    {
        object dataCopy = data;

        if (type == UserMetricsType.SleepSessionRecords || type == UserMetricsType.StepsRecords) {
            dataCopy = JsonConvert.SerializeObject(data);
        }
        var metricSetEvent = new MetricSetEvent(type, dataCopy);

        AnalyticsService.Instance.RecordEvent(metricSetEvent);
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
        AnalyticsService.Instance.RecordEvent(gameSummaryEvent);
    }
}