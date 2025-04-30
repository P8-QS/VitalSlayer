using System;
using Metrics;
using UnityEngine;

namespace Effects
{
    public class EnemyVariabilityEffect : IEffect
    {
        public string Name => "Enemy Variability";
        public int Level { get; set; }
        public Sprite Icon { get; }

        public EnemyVariabilityEffect(Sprite icon, int level)
        {
            Icon = icon;
            Level = level;
        }
        
        public string Text()
        {
            return $"<color=#F5CC45>enemy variability level {Level}</color>";
        }

        public string Description()
        {
            return (HeartRateVariabilityMetric.Hrv)Level switch
            {
                HeartRateVariabilityMetric.Hrv.Low => "Enemies have low variability",
                HeartRateVariabilityMetric.Hrv.Normal => "Enemy variability is not affected",
                HeartRateVariabilityMetric.Hrv.High => "Enemies have high variability",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Apply()
        {
            EntitySpawner.Instance.DistributionLevel = Level;
        }
    }
}