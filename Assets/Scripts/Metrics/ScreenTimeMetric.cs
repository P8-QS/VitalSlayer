using Data;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class ScreenTimeMetric : IMetric
    {
        private long _data;
        private IEffect _effect;
        private Sprite _icon;
        public string Name => "Screen Time";
        public long Data
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

        public ScreenTimeMetric()
        {
            Data = UserMetricsHandler.Instance.TotalScreenTime;

            Icon = SpriteManager.Instance.GetSprite("metric_screen_time");

            int effectLevel = (Data / (1000 * 60 * 60)) switch
            {
                > 4 => 2,
                > 2 => 1,
                _ => 0
            };

            if (effectLevel > 0)
            {
                _effect = new FogEffect(SpriteManager.Instance.GetSprite("effect_fog"), effectLevel);
            }
            else
            {
                _effect = new ScoutingEffect(SpriteManager.Instance.GetSprite("effect_scouting"), effectLevel);
            }
        }
        public string Text()
        {
            long hours = Data / (1000 * 60 * 60);
            long minutes = Data / (1000 * 60) % 60;
            return $"You have spent <b>{hours} hours and {minutes} minutes</b> on your phone. This gives you {Effect.Text()}.";
        }
        public string Description()
        {
            long hours = Data / (1000 * 60 * 60);
            long minutes = Data / (1000 * 60) % 60;
            return $"You have spent {hours} hours and {minutes} minutes on your phone yesterday. Consider balancing screen time for better focus.";
        }
    }
}