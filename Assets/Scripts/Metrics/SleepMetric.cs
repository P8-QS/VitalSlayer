using System;
using System.Linq;
using Data;
using Data.Models;
using Effects;
using UnityEngine;

namespace Metrics {
    public class SleepMetric : IMetric<SleepSessionRecord>
    {
        public string Name => "Sleep";

        private SleepSessionRecord _data;
        public SleepSessionRecord Data
        {
            get => _data ?? UserMetricsHandler.Instance.SleepSessionRecords.First();
            set => _data = value;
        }
        public IEffect Effect => new SleepEffect(); 
        public Sprite Icon => throw new NotImplementedException();
        public override string ToString()
        {
            return $"{Name} level {Effect.Level}";
        }
        public string Text()
        {
            throw new NotImplementedException();
        }
        public string Description()
        {
            throw new System.NotImplementedException();
        }

    }
}