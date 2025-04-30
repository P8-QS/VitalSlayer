using System.Collections.Generic;
using System.Linq;
using Data;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class ScreenTimeMetric : IMetric
    {
        public string Name => "Screen Time";
        public List<IEffect> Effects { get; } = new();
        public long Data { get; }
        public Sprite Icon { get; }

        public ScreenTimeMetric()
        {
            Data = UserMetricsHandler.Instance.TotalScreenTime;
            Icon = SpriteManager.Instance.GetSprite("metric_screen_time");

            var effectLevel = (Data / (1000 * 60 * 60)) switch
            {
                > 4 => 2,
                > 2 => 1,
                _ => 0
            };

            if (effectLevel > 0)
            {
                Effects.Add(new FogEffect(SpriteManager.Instance.GetSprite("effect_fog"), effectLevel));
            }
            else
            {
                Effects.Add(new ScoutingEffect(SpriteManager.Instance.GetSprite("effect_scouting"), effectLevel));
            }
        }

        public string Text()
        {
            var hours = Data / (1000 * 60 * 60);
            var minutes = Data / (1000 * 60) % 60;
            return
                $"You have spent <b>{hours} hours and {minutes} minutes</b> on your phone. This gives you {Effects.Select(e => e.Text()).Aggregate((a, b) => $"{a}, {b}")}.";
        }

        public string Description()
        {
            var hours = Data / (1000 * 60 * 60);
            var minutes = Data / (1000 * 60) % 60;
            return
                $"You have spent {hours} hours and {minutes} minutes on your phone yesterday. Consider balancing screen time for better focus.";
        }
    }
}