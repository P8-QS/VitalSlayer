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
        public StepsRecord Data { get; }
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        public StepsMetric()
        {
            if (UserMetricsHandler.Instance.StepsRecords != null)
            {
                Data = UserMetricsHandler.Instance.StepsRecords.FirstOrDefault();
                Icon = SpriteManager.Instance.GetSprite("metric_steps");

                if (Data is null) return;
                var effectLevel = Data.StepsCount switch
                {
                    < 4000 => 1,
                    < 8000 => 2,
                    _ => 3
                };

                Effects.Add(new MapEffect(SpriteManager.Instance.GetSprite("effect_map"), effectLevel));
                Effects[0].Apply(); // Scuffed API usage but works
            }
            else
            {
                Data = null;
            }
        }

        public string Text()
        {
            string formattedSteps = Data.StepsCount.ToString("N0");
            return
                $"You have taken <b>{formattedSteps} steps</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            string formattedSteps = Data.StepsCount.ToString("N0");
            return $"You have taken {formattedSteps} steps yesterday, contributing to your movement activity.";
        }
    }
}