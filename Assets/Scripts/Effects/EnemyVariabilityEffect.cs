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
            return Level switch
            {
                1 => "<color=#EA2E1E>low enemy variability level</color>",
                2 => "<color=#F5CC45>some enemy variability level</color>",
                3 => "<color=#3AE75B>high enemy variability level</color>",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string Description()
        {
            return (HeartRateVariabilityMetric.Hrv)Level switch
            {
                HeartRateVariabilityMetric.Hrv.Low => "Enemies health and damage will have low variability.",
                HeartRateVariabilityMetric.Hrv.Normal => "Enemy health and damage will have some variability.",
                HeartRateVariabilityMetric.Hrv.High => "Enemies health and damage have high variability.",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Apply()
        {
            EntitySpawner.Instance.DistributionLevel = Level;
        }
    }
}