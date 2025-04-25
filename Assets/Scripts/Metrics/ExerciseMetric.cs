using Effects;
using UnityEngine;

namespace Metrics
{
    public class ExerciseMetric : IMetric
    {
        public string Name => "Exercise";
        public IEffect Effect { get; }
        public Sprite Icon { get; }
     
        public ExerciseMetric()
        {
            throw new System.NotImplementedException();
        }
        
        public string Text()
        {
            throw new System.NotImplementedException();
        }

        public string Description()
        {
            throw new System.NotImplementedException();
        }
    }
}