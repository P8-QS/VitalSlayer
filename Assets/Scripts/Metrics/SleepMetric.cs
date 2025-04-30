using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class SleepMetric : IMetric
    {
        public string Name => "Sleep";
        public IReadOnlyCollection<SleepSessionRecord> Data { get; }
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        private readonly TimeSpan _sleepDuration;

        public SleepMetric()
        {
            if (UserMetricsHandler.Instance.SleepSessionRecords is null) return;
            Data = UserMetricsHandler.Instance.SleepSessionRecords.OrderByDescending(s => s.StartTime).ToList();
            Icon = SpriteManager.Instance.GetSprite("metric_sleep");

            if (Data is null) return;
            _sleepDuration = new TimeSpan(Data.Sum(s => s.Duration.Ticks));
            switch (_sleepDuration.TotalHours)
            {
                case < 5:
                    Effects.Add(new HallucinationEffect(SpriteManager.Instance.GetSprite("effect_hallucination"),
                        2));
                    Effects.Add(new LevelIndicatorEffect(
                        SpriteManager.Instance.GetSprite("effect_level_indicator_negative"),
                        0));
                    break;
                case < 7:
                    Effects.Add(new HallucinationEffect(SpriteManager.Instance.GetSprite("effect_hallucination"),
                        0));
                    Effects.Add(new LevelIndicatorEffect(
                        SpriteManager.Instance.GetSprite("effect_level_indicator_positive"),
                        1));
                    break;
                default:
                    Effects.Add(new LevelIndicatorEffect(
                        SpriteManager.Instance.GetSprite("effect_level_indicator_positive"),
                        1)); break;
            }
        }

        public string Text()
        {
            return
                $"You have slept <b>{_sleepDuration.Hours} hours and {_sleepDuration.Minutes} minutes</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            return
                $"You slept for {_sleepDuration.Hours} hours and {_sleepDuration.Minutes} minutes yesterday. Proper rest is essential for recovery and focus.";
        }
    }
}