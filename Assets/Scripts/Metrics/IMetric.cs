using Effects;
using UnityEngine;

namespace Metrics
{
    public interface IMetric<T>
    {
        public string Name { get; }
        public T Data { get; }
        public IEffect Effect { get; }
        public Sprite Icon { get; }
        public string Text();
        public string Description();
    }
}