using System;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics {
    public class StepsMetric: IMetric<StepsRecord>
    {
        private StepsRecord _data; 
        public string Name => "Steps";
        public StepsRecord Data
        {
            get => _data;
            set => _data = value;
        }
        public IEffect Effect => new MapEffect(); 
        public Sprite Icon => throw new NotImplementedException();

        public StepsMetric() {
            var stepsRecords = UserMetricsHandler.Instance?.StepsRecords;
            if (stepsRecords == null)
            {
                Debug.Log("INSTANCE WAS NULL");
            }
            else if (!stepsRecords.Any()) {
                Debug.Log("There were no stepsrecords");
            }
            
            Effect.Level = Data.StepsCount switch
            {
                < 4000 => 1,
                < 8000 => 2,
                _ => 3
            };
        }
        public override string ToString()
        {
            return $"{Name} level {Effect.Level}";
        }
        public string Text()
        {
            return $"map size {Effect.Level}";
        }
        public string Description()
        {
            string formattedSteps = Data.StepsCount.ToString("N0");

            return $"You have taken {formattedSteps} steps. This gives you {Effect.Text()}";
        }

    }
}