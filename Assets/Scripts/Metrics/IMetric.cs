using Effects;
using UnityEngine;

namespace Metrics
{
    public interface IMetric
    {
        public string Name { get; }
        public IEffect Effect { get; }
        public Sprite Icon { get; }
        public string Text();
        public string Description();
    }
}