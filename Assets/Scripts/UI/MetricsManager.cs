using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using Data;
using UI;

public class MetricsManager : MonoBehaviour
{
    public GameObject metricCardPrefab;  // Assign the MetricCard prefab in Unity Inspector
    public Transform parentPanel;  // The UI Panel that holds all metric cards
    public Transform modalParent;  // The UI Panel that holds all modals

    public SpriteAtlas spriteAtlas;

    void Start()
    {
        Debug.Log("MetricsManager is running!");

        // var steps = UserMetricsHandler.Instance.StepsRecords.FirstOrDefault()?.StepsCount ?? 0;
        // var sleepSessions = UserMetricsHandler.Instance.SleepSessionRecords.FirstOrDefault();
        // var screenTime = UserMetricsHandler.Instance.ScreenTimeRecords.FirstOrDefault();

        int steps = 6000;
        var screenTime = new TimeSpan(2, 30, 0);
        
        // int sleepHours = 0, sleepMinutes = 0, screenHours = 0, screenMinutes = 0;
        
        // if (sleepSessions != null)
        // {
        //     var totalSleep = sleepSessions.EndTime - sleepSessions.StartTime;
        //     sleepHours = (int)totalSleep.TotalHours;
        //     sleepMinutes = totalSleep.Minutes;
        // }

        var totalSleep = new TimeSpan(0, 6, 33);
        int sleepHours = (int)totalSleep.TotalHours;
        int sleepMinutes = totalSleep.Minutes;
        
        // if (screenTime != null)
        // {
        //     screenHours = (int)screenTime.Duration.TotalHours;
        //     screenMinutes = screenTime.Duration.Minutes;
        // }
        
        int screenHours = (int)screenTime.TotalHours;
        int screenMinutes = screenTime.Minutes;

        // Load metric icons
        Sprite sleepIcon = spriteAtlas.GetSprite("metric_sleep");
        Sprite stepsIcon = spriteAtlas.GetSprite("metric_steps");
        Sprite screenTimeIcon = spriteAtlas.GetSprite("metric_screen_time");

        // Load effect icons
        Sprite fogIcon = spriteAtlas.GetSprite("effect_fog");
        Sprite hallucinationIcon = spriteAtlas.GetSprite("effect_hallucination");
        Sprite attackSpeedIcon = spriteAtlas.GetSprite("effect_attack_speed");
        Sprite mapSizeIcon = spriteAtlas.GetSprite("effect_map");

        bool hasMetrics = false;

        // **Sleep Metric**
        // if (sleepHours > 0 || sleepMinutes > 0)
        // {
        //     string sleepText = MetricsStringGenerator.Sleep(sleepHours, sleepMinutes);
        //     string sleepDescription = MetricsStringGenerator.SleepDescription(sleepHours, sleepMinutes);
        //
        //     int hallucinationLevel = sleepHours < 5 ? 2 : (sleepHours < 7 ? 1 : 0);
        //     Sprite[] sleepEffects = hallucinationLevel > 0 ? new[] { hallucinationIcon } : new[] { attackSpeedIcon };
        //
        //     AddMetric(sleepIcon, "Sleep", sleepText, sleepEffects);
        //     hasMetrics = true;
        // }

        // **Steps Metric**
        if (steps > 0)
        {
            string stepsText = MetricsStringGenerator.Steps(steps);
            string stepsDescription = MetricsStringGenerator.StepsDescription(steps);

            AddMetric(stepsIcon, "Steps", stepsText, new[] { mapSizeIcon });
            hasMetrics = true;
        }

        // **Screen Time Metric**
        if (screenHours > 0 || screenMinutes > 0)
        {
            string screenText = MetricsStringGenerator.ScreenTime(screenHours, screenMinutes);
            string screenDescription = MetricsStringGenerator.ScreenTimeDescription(screenHours, screenMinutes);

            int fogLevel = screenHours < 2 ? 0 : (screenHours < 4 ? 1 : 2);
            Sprite[] screenEffects = fogLevel > 0 ? new[] { fogIcon } : null;

            AddMetric(screenTimeIcon, "Screen Time", screenText, screenEffects);
            hasMetrics = true;
        }

        // Show error message if no metrics are available
        if (!hasMetrics)
        {
            ShowErrorText("No metrics available.");
        }
    }

    void AddMetric(Sprite icon, string title, string description, [CanBeNull] Sprite[] effectIcons = null)
    {
        GameObject newCard = Instantiate(metricCardPrefab, parentPanel);
        MetricCardUI metricUI = newCard.GetComponent<MetricCardUI>();
        metricUI.modalParent = modalParent;
        metricUI.SetMetric(icon, title, description, effectIcons);
    }

    void ShowErrorText(string message)
    {
        GameObject go = new GameObject("ErrorText");
        go.transform.SetParent(parentPanel);
        var errorText = go.AddComponent<TextMeshProUGUI>();
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MinecraftRegular SDF");
        errorText.fontSize = 36;
        errorText.text = message;
    }
}
