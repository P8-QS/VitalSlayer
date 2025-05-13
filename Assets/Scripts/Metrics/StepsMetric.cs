using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class StepsMetric : IMetric
    {
        public string Name => "Steps";
        public IReadOnlyCollection<StepsRecord> Data { get; }
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }
        private readonly int _stepsCount;

        public StepsMetric()
        {
            if (UserMetricsHandler.Instance.StepsRecords is null) return;
            Data = UserMetricsHandler.Instance.StepsRecords;
            Icon = SpriteManager.Instance.GetSprite("metric_steps");

            _stepsCount = Data.Sum(d => d.StepsCount);
            
            if (Data is null) return;
            var effectLevel = _stepsCount switch
            {
                < 4000 => 1,
                < 8000 => 2,
                _ => 3
            };

            Effects.Add(new MapEffect(SpriteManager.Instance.GetSprite("effect_map"), effectLevel));
            Effects[0].Apply(); // Scuffed API usage but works
        }

        public string Text()
        {
            string formattedSteps = _stepsCount.ToString("N0");
            return
                $"You have taken <b>{formattedSteps} steps</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            string formattedSteps = _stepsCount.ToString("N0");
            return $"You have taken {formattedSteps} steps yesterday, contributing to your movement activity.";
        }
    }
}