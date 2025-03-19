using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics {
    public class StepsMetric: IMetric
    {
        private StepsRecord _data;
        private IEffect _effect;
        private Sprite _icon;
        public string Name => "Steps";
        public StepsRecord Data
        {
            get => _data;
            set => _data = value;
        }
        public IEffect Effect { get => _effect; }
        public Sprite Icon
        {
            get => _icon;
            set => _icon = value;
        }

        public StepsMetric() {
            Data = UserMetricsHandler.Instance.StepsRecords.FirstOrDefault();
            
            Icon = SpriteManager.Instance.GetSprite("metric_steps");

            int effectLevel = Data.StepsCount switch
            {
                < 4000 => 1,
                < 8000 => 2,
                _ => 3
            };

            _effect = new MapEffect(SpriteManager.Instance.GetSprite("effect_map"), effectLevel);
        }
        public string Text()
        {
            string formattedSteps = Data.StepsCount.ToString("N0");
            return $"You have taken {formattedSteps} steps. This gives you {Effect.Text()}.";
        }
        public string Description()
        {
            string formattedSteps = Data.StepsCount.ToString("N0");
            return $"You have taken {formattedSteps} steps yesterday, contributing to your movement activity.";
        }
    }
}