using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class Vo2MaxMetric : IMetric
    {
        public IReadOnlyCollection<Vo2MaxRecord> Data { get; }
        public string Name => "VO2 Max";
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }

        private Vo2MaxRecord _record;
        private int level;

        public Vo2MaxMetric()
        {
            Data = UserMetricsHandler.Instance.Vo2MaxRecords;
            Icon = SpriteManager.Instance.GetSprite("metric_vo2_max");

            if (!(Data?.Count > 0)) return;

            _record = Data.OrderByDescending(record => record.Time).FirstOrDefault();

            if (_record == null) return;

            level = _record.Vo2MillilitersPerMinuteKilogram switch
            {
                > 45 => 2,
                > 35 => 1,
                _ => 0
            };
            
            if (level == 2)
            {
                Effects.Add(new DodgeSlimeAcidEffect(SpriteManager.Instance.GetSprite("effect_slime_puddle_positive"), 2));
            } else if (level == 1)
            {
                Effects.Add(new DodgeSlimeAcidEffect(SpriteManager.Instance.GetSprite("effect_slime_puddle_neutral"), 1));
            }
            else
            {
                Effects.Add(new DodgeSlimeAcidEffect(SpriteManager.Instance.GetSprite("effect_slime_puddle_negative"), 0));
            }
        }

        public string Text()
        {
            return
                $"Your VO2 max is <b>{_record.Vo2MillilitersPerMinuteKilogram} (mL/kg/min)</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        private string LevelToString()
        {
            return level switch
            {
                0 => "below average",
                1 => "average",
                2 => "above average",
                _ => "unknown"
            };
        }

        public string Description()
        {
            return
                $"Your VO2 max is <b>{_record.Vo2MillilitersPerMinuteKilogram} (mL/kg/min)</b>. This is considered {LevelToString()}. VO2 max is a measure of your body's ability to utilize oxygen during exercise. ";
        }
    }
}