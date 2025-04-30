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
        public HeartRateVariabilityRmssdRecord Data { get; }
        public string Name => "Heart Rate Variability";
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        public HeartRateVariabilityMetric()
        {
            Data = UserMetricsHandler.Instance.HeartRateVariabilityRmssdRecords.OrderByDescending(d => d.Time).FirstOrDefault();
            Icon = SpriteManager.Instance.GetSprite("metric_hrv");

            if (Data is null) return;
            var effectLevel = Data.HeartRateVariabilityMillis switch
            {
                > 70 => Hrv.High,
                > 30 => Hrv.Normal,
                _ => Hrv.Low
            };

            Effects.Add(new EnemyVariabilityEffect(SpriteManager.Instance.GetSprite("effect_enemy_spawn"), (int)effectLevel));
        }
        
        public string Text()
        {
            return $"Your heart rate variability is <b>{Data.HeartRateVariabilityMillis}</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            return $"Your heart rate variability is {Data.HeartRateVariabilityMillis}.";
        }

        public enum Hrv
        {
            Low = 1,
            Normal = 2,
            High = 3
        }
    }
}