using System.Collections.Generic;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class ActiveCaloriesMetric : IMetric
    {
        private IReadOnlyCollection<ActiveCaloriesBurnedRecord> _data;
        private Sprite _icon;

        public string Name => "Active Calories Burned";

        public IReadOnlyCollection<ActiveCaloriesBurnedRecord> Data
        {
            get => _data;
            private set => _data = value;
        }

        public List<IEffect> Effects { get; } = new();

        public Sprite Icon
        {
            get => _icon;
            private set => _icon = value;
        }

        private int _totalCalories;

        public ActiveCaloriesMetric()
        {
            Data = UserMetricsHandler.Instance.ActiveCaloriesBurnedRecords;
            Icon = SpriteManager.Instance.GetSprite("metric_calories");

            //_totalCalories = 0;
            foreach (var record in Data)
            {
                if (record.Energy != null)
                {
                    _totalCalories += (int)record.Energy.Value / 1000;
                }
            }


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
                $"You burned <b>{_totalCalories} active calories</b> today. This gives you {(this as IMetric).EffectsToString()}.";
        }

        public string Description()
        {
            return $"You've burned a total of {_totalCalories} active calories. " +
                   $"Staying physically active improves endurance, mood, and overall health.";
        }
    }
}