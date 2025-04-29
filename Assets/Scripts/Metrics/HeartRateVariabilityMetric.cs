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

        public HeartRateVariabilityMetric()
        {
            Data = UserMetricsHandler.Instance.HeartRateVariabilityRmssdRecords;
            Icon = SpriteManager.Instance.GetSprite("metric_hrv");
            
            if (!(Data?.Count > 0)) return;
            var effectLevel = Data.OrderByDescending(d => d.Time).FirstOrDefault()?.HeartRateVariabilityMillis switch
            {
                > 70 => Hrv.High,
                > 30 => Hrv.Normal,
                _ => Hrv.Low
            };

            if (effectLevel is Hrv.Low or Hrv.High)
            {
                Effects.Add(new EnemyVariabilityEffect(SpriteManager.Instance.GetSprite("effect_enemy_spawn"), (int)effectLevel));
            }
        }
        
        public string Text()
        {
            throw new System.NotImplementedException();
        }

        public string Description()
        {
            throw new System.NotImplementedException();
        }

        public enum Hrv
        {
            Low,
            Normal,
            High
        }
    }
}