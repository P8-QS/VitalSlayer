using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics {
    public class SleepMetric : IMetric
    {
        public string Name => "Sleep";
        public IReadOnlyCollection<SleepSessionRecord> Data { get; }
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        private readonly TimeSpan _sleepDuration;
        
        public SleepMetric() 
        {
            Data = UserMetricsHandler.Instance.SleepSessionRecords.OrderByDescending(s => s.StartTime).ToList();
            Icon = SpriteManager.Instance.GetSprite("metric_sleep");

            if (Data is null) return;
            _sleepDuration = new TimeSpan(Data.Sum(s => s.Duration.Ticks));
            var effectLevel = _sleepDuration.TotalHours switch
            {
                < 5 => 2,
                < 7 => 1,
                _ => 0
            };

            if (effectLevel > 0) 
            {
                Effects.Add(new HallucinationEffect(SpriteManager.Instance.GetSprite("effect_hallucination"),
                    effectLevel));
            }
            else 
            {
                Effects.Add(new AttackSpeedEffect(SpriteManager.Instance.GetSprite("effect_attack_speed"), effectLevel));
            }
        }
        public string Text()
        {
            return $"You have slept <b>{_sleepDuration.Hours} hours and {_sleepDuration.Minutes} minutes</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }
        public string Description()
        {
            return $"You slept for {_sleepDuration.Hours} hours and {_sleepDuration.Minutes} minutes yesterday. Proper rest is essential for recovery and focus.";
        }
    }
}