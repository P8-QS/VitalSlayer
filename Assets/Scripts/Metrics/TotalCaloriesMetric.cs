using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class TotalCaloriesMetric : IMetric
    {
        private IReadOnlyCollection<TotalCaloriesBurnedRecord> _data;

        public string Name => "Total Calories Burned";
        public IReadOnlyCollection<TotalCaloriesBurnedRecord> Data { get; }
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        private readonly int _totalCalories;

        public TotalCaloriesMetric()
        {
            if (UserMetricsHandler.Instance.TotalCaloriesBurnedRecords is null) return;
            
            Data = UserMetricsHandler.Instance.TotalCaloriesBurnedRecords;
            
            Icon = SpriteManager.Instance.GetSprite("metric_calories");

            _totalCalories = Data.Sum(c => (int)c.Energy.Value / 1000);
            
            switch (_totalCalories)
            {
                case >= 300:
                    Effects.Add(new NoDoorCloseEffect(SpriteManager.Instance.GetSprite("effect_no_doors_positive"),
                        1)); break;
                default:
                    Effects.Add(new NoDoorCloseEffect(SpriteManager.Instance.GetSprite("effect_no_doors_negative"),
                        0)); break;
            }
        }

        public string Text()
        {
            return
                $"You burned <b>{_totalCalories} calories</b> yesterday. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            return $"You've burned a total of {_totalCalories} calories. " +
                   $"Staying physically active improves endurance, mood, and overall health.";
        }
    }
}