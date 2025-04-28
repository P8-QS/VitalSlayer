using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics {
    public class SleepMetric : IMetric
    {
        private SleepSessionRecord _data;
        private IEffect _effect;
        private Sprite _icon;
        public string Name => "Sleep";
        public SleepSessionRecord Data
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

        public SleepMetric() {
            if (UserMetricsHandler.Instance.SleepSessionRecords != null) {
                Data = UserMetricsHandler.Instance.SleepSessionRecords.FirstOrDefault();
                
                Icon = SpriteManager.Instance.GetSprite("metric_sleep");

                int effectLevel = Data.Duration.TotalHours switch
                {
                    < 5 => 2,
                    < 7 => 1,
                    _ => 0
                };

                if (effectLevel > 0) 
                {
                    _effect = new HallucinationEffect(SpriteManager.Instance.GetSprite("effect_hallucination"), effectLevel);
                }
                else 
                {
                    _effect = new AttackSpeedEffect(SpriteManager.Instance.GetSprite("effect_attack_speed"), effectLevel);
                }
            } 
            else
            {
                Data = null;
            }
        }
        public string Text()
        {
            return $"You have slept <b>{Data.Duration.TotalHours} hours and {Data.Duration.Minutes} minutes</b>. This gives you {Effect.Text()}.";
        }
        public string Description()
        {
            return $"You slept for {Data.Duration.TotalHours} hours and {Data.Duration.Minutes} minutes yesterday. Proper rest is essential for recovery and focus.";
        }
    }
}