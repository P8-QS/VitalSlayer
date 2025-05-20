using System;
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

        private readonly TimeSpan _exerciseDuration;
     
        public ExerciseMetric()
        {
            if (UserMetricsHandler.Instance.ExerciseSessionRecords is null) return;
            
            Data = UserMetricsHandler.Instance.ExerciseSessionRecords
                .OrderByDescending(e => e.StartTime)
                .ToList();
            
            Icon = SpriteManager.Instance.GetSprite("metric_exercise");

            if (!(Data?.Count > 0)) return;
            
            _exerciseDuration = new TimeSpan(Data.Sum(e => e.Duration.Ticks));
            switch (_exerciseDuration.TotalMinutes)
            {
                case > 30:
                    Effects.Add(new DodgeTrapsEffect(SpriteManager.Instance.GetSprite("effect_dodge_traps"), 0));
                    Effects.Add(new CombatInfoEffect(SpriteManager.Instance.GetSprite("effect_combat_info_positive"), 1));
                    break;

                case > 1:
                    Effects.Add(new CombatInfoEffect(SpriteManager.Instance.GetSprite("effect_combat_info_positive"), 1)); break;
                default:
                    Effects.Add(new CombatInfoEffect(SpriteManager.Instance.GetSprite("effect_combat_info_negative"), 2)); break;
            }
        }

        public string Text()
        {
            return $"You have exercised for <b>{_exerciseDuration.TotalMinutes:0.##} minutes</b>. This gives you {(this as IMetric).EffectsToString()}.";
        }
        
        public string Description()
        {
            return $"You have exercised for {_exerciseDuration.TotalMinutes:0.##} minutes yesterday.";
        }
    }
}