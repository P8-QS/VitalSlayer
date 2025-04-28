using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics
{
    public class ExerciseMetric : IMetric
    {
        public IReadOnlyCollection<ExerciseSessionRecord> Data { get; }
        public string Name => "Exercise";
        public List<IEffect> Effects { get; } = new();
        public Sprite Icon { get; }
     
        public ExerciseMetric()
        {
            Data = UserMetricsHandler.Instance.ExerciseSessionRecords;
            Icon = SpriteManager.Instance.GetSprite("metric_exercise");

            if (!(Data?.Count > 0)) return;
            var effectLevel = Data.Sum(exercise => exercise.Duration.TotalMinutes) switch
            {
                > 30 => 2,
                > 1 => 1,
                _ => 0
            };

            if (effectLevel > 0)
            {
                Effects.Add(new DodgeTrapsEffect(SpriteManager.Instance.GetSprite("effect_dodge_traps"), 1));
            }
        }

        public string Text()
        {
            return $"You have exercised for <b>{Data.Sum(e => e.Duration.TotalMinutes)} minutes</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }

        

        public string Description()
        {
            return $"You have exercised for {Data.Sum(e => e.Duration.TotalMinutes)} minutes yesterday.";
        }
    }
}