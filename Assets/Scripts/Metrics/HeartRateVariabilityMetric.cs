using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class HeartRateVariabilityMetric : IMetric
    {
        public IReadOnlyCollection<HeartRateVariabilityRmssdRecord> Data { get; }
        public string Name => "Heart Rate Variability";
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        private decimal _heartRateVariability;

        public HeartRateVariabilityMetric()
        {
            if (UserMetricsHandler.Instance.HeartRateVariabilityRmssdRecords is null) return;
            Data = UserMetricsHandler.Instance.HeartRateVariabilityRmssdRecords;
            if (Data.Count == 0) return;

            // Yesterday average
            var yesterday = System.DateTime.Now.AddDays(-1).Date;
            var yesterdayData = Data.Where(r => r.Time.Date == yesterday).ToList();
            _heartRateVariability = yesterdayData.Average(r => r.HeartRateVariabilityMillis);


            Icon = SpriteManager.Instance.GetSprite("metric_hrv");

            if (Data is null) return;
            var effectLevel = _heartRateVariability switch
            {
                > 70 => Hrv.High,
                > 30 => Hrv.Normal,
                _ => Hrv.Low
            };

            Effects.Add(new EnemyVariabilityEffect(SpriteManager.Instance.GetSprite("effect_enemy_spawn"),
                (int)effectLevel));
        }

        public string Text()
        {
            return
                $"Your average heart rate variability was <b>{_heartRateVariability}</b> yesterday. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            return $"Your average heart rate variability was {_heartRateVariability} yesterday.";
        }

        public enum Hrv
        {
            Low = 1,
            Normal = 2,
            High = 3
        }
    }
}